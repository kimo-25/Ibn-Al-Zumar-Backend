using IbnAlZumar.API.Common.Exceptions;
using IbnAlZumar.API.Common.Settings;
using IbnAlZumar.API.DTOs.Common;
using IbnAlZumar.API.DTOs.Sales;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.API.Services.Notifications;
using IbnAlZumar.Domain.Entities.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace IbnAlZumar.API.Services.Sales;

public class CustomerDebtService : ICustomerDebtService
{
    private readonly ApplicationDbContext _context;
    private readonly INotificationComposer _composer;
    private readonly INotificationSender _sender;
    private readonly DebtReminderOptions _options;

    public CustomerDebtService(
        ApplicationDbContext context,
        INotificationComposer composer,
        INotificationSender sender,
        IOptions<DebtReminderOptions> options)
    {
        _context = context;
        _composer = composer;
        _sender = sender;
        _options = options.Value;
    }

    public async Task<PagedResultDto<CustomerDebtDto>> GetDebtDashboardAsync(int pageNumber, int pageSize, CancellationToken ct = default)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize is < 1 or > 200 ? 30 : pageSize;

        var query = _context.Customers
            .AsNoTracking()
            .Where(c => c.CurrentBalance > 0)
            .OrderByDescending(c => c.CurrentBalance);

        var totalCount = await query.CountAsync(ct);

        var customerIds = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => c.Id)
            .ToListAsync(ct);

        var customers = await _context.Customers
            .AsNoTracking()
            .Where(c => customerIds.Contains(c.Id))
            .ToListAsync(ct);

        var schedules = await _context.CustomerDebtSchedules
            .AsNoTracking()
            .Where(s => customerIds.Contains(s.CustomerId))
            .ToDictionaryAsync(s => s.CustomerId, ct);

        // NOTE: CustomerLedgerEntry's exact field for "when" wasn't confirmed while building this
        // (ARCHITECTURE.md's field list doesn't explicitly show a TransactionDate on it the way it
        // does for SupplierLedgerEntry) — CreatedAt (guaranteed by BaseEntity) is used instead. If
        // your CustomerLedgerEntry does have a TransactionDate, swap it in below for accuracy.
        var lastPayments = await _context.CustomerLedgerEntries
            .AsNoTracking()
            .Where(l => customerIds.Contains(l.CustomerId) && l.RelatedPaymentId != null)
            .GroupBy(l => l.CustomerId)
            .Select(g => new { CustomerId = g.Key, LastDate = g.Max(x => x.CreatedAt) })
            .ToDictionaryAsync(x => x.CustomerId, x => x.LastDate, ct);

        var items = customers
            .OrderByDescending(c => c.CurrentBalance)
            .Select(c =>
            {
                schedules.TryGetValue(c.Id, out var schedule);
                lastPayments.TryGetValue(c.Id, out var lastPaymentDate);

                return new CustomerDebtDto
                {
                    CustomerId = c.Id,
                    CustomerName = c.FullName,
                    CustomerPhone = c.Phone,
                    CustomerEmail = c.Email,
                    CurrentBalance = c.CurrentBalance,
                    CreditLimit = c.CreditLimit,
                    LastPaymentDate = lastPaymentDate == default ? null : lastPaymentDate,
                    IsScheduleActive = schedule?.IsActive ?? false,
                    LastReminderSentAt = schedule?.LastReminderSentAt,
                    NextReminderDueAt = schedule?.NextReminderDueAt,
                    ReminderCount = schedule?.ReminderCount ?? 0
                };
            })
            .ToList();

        return new PagedResultDto<CustomerDebtDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task ReconcileSchedulesAsync(CancellationToken ct = default)
    {
        var indebtedCustomerIds = await _context.Customers
            .Where(c => c.CurrentBalance > _options.MinimumBalanceToRemind)
            .Select(c => c.Id)
            .ToListAsync(ct);

        var existingSchedules = await _context.CustomerDebtSchedules.ToListAsync(ct);
        var existingByCustomer = existingSchedules.ToDictionary(s => s.CustomerId);

        foreach (var customerId in indebtedCustomerIds)
        {
            if (existingByCustomer.TryGetValue(customerId, out var schedule))
            {
                if (!schedule.IsActive)
                {
                    schedule.IsActive = true;
                    schedule.NextReminderDueAt = DateTime.UtcNow; // due immediately on reactivation
                }
            }
            else
            {
                _context.CustomerDebtSchedules.Add(new CustomerDebtSchedule
                {
                    CustomerId = customerId,
                    IsActive = true,
                    NextReminderDueAt = DateTime.UtcNow
                });
            }
        }

        foreach (var schedule in existingSchedules.Where(s => s.IsActive && !indebtedCustomerIds.Contains(s.CustomerId)))
        {
            schedule.IsActive = false;
        }

        await _context.SaveChangesAsync(ct);
    }

    public async Task<int> SendDueRemindersAsync(CancellationToken ct = default)
    {
        var dueSchedules = await _context.CustomerDebtSchedules
            .Include(s => s.Customer)
            .Where(s => s.IsActive && s.NextReminderDueAt <= DateTime.UtcNow)
            .ToListAsync(ct);

        var sentCount = 0;

        foreach (var schedule in dueSchedules)
        {
            var sent = await SendReminderAsync(schedule, ct);
            if (sent) sentCount++;

            // Advance regardless of send success — a persistently-unreachable phone shouldn't
            // retry every single job run forever; it'll still show up on the dashboard as
            // "reminder overdue" via NextReminderDueAt being in the past for staff to notice.
            schedule.LastReminderSentAt = DateTime.UtcNow;
            schedule.NextReminderDueAt = DateTime.UtcNow.AddDays(_options.IntervalDays);
            schedule.ReminderCount += 1;
            schedule.LastKnownBalance = schedule.Customer.CurrentBalance;
        }

        await _context.SaveChangesAsync(ct);
        return sentCount;
    }

    public async Task<CustomerDebtDto> SendReminderNowAsync(int customerId, CancellationToken ct = default)
    {
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == customerId, ct)
            ?? throw new NotFoundException("العميل غير موجود.");

        if (customer.CurrentBalance <= 0)
            throw new BadRequestException("لا يوجد رصيد مستحق على هذا العميل.");

        var schedule = await _context.CustomerDebtSchedules.FirstOrDefaultAsync(s => s.CustomerId == customerId, ct);
        if (schedule is null)
        {
            schedule = new CustomerDebtSchedule { CustomerId = customerId, IsActive = true };
            _context.CustomerDebtSchedules.Add(schedule);
        }

        await SendReminderAsync(schedule, ct, customer);

        schedule.IsActive = true;
        schedule.LastReminderSentAt = DateTime.UtcNow;
        schedule.NextReminderDueAt = DateTime.UtcNow.AddDays(_options.IntervalDays);
        schedule.ReminderCount += 1;
        schedule.LastKnownBalance = customer.CurrentBalance;

        await _context.SaveChangesAsync(ct);

        var dashboard = await GetDebtDashboardAsync(1, 1, ct); // cheap re-fetch pattern kept consistent with other services' GetByIdAsync-after-write style
        return new CustomerDebtDto
        {
            CustomerId = customer.Id,
            CustomerName = customer.FullName,
            CustomerPhone = customer.Phone,
            CustomerEmail = customer.Email,
            CurrentBalance = customer.CurrentBalance,
            CreditLimit = customer.CreditLimit,
            IsScheduleActive = schedule.IsActive,
            LastReminderSentAt = schedule.LastReminderSentAt,
            NextReminderDueAt = schedule.NextReminderDueAt,
            ReminderCount = schedule.ReminderCount
        };
    }

    private async Task<bool> SendReminderAsync(CustomerDebtSchedule schedule, CancellationToken ct, Customer? loadedCustomer = null)
    {
        var customer = loadedCustomer ?? schedule.Customer;

        var lastPaymentDate = await _context.CustomerLedgerEntries
            .Where(l => l.CustomerId == customer.Id && l.RelatedPaymentId != null)
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => (DateTime?)l.CreatedAt)
            .FirstOrDefaultAsync(ct);

        var anySent = false;

        if (!string.IsNullOrWhiteSpace(customer.Phone))
        {
            var message = _composer.ComposeDebtReminderWhatsApp(customer, customer.CurrentBalance, lastPaymentDate);
            var sent = await _sender.SendWhatsAppTemplateAsync(message, "CustomerDebtSchedule", schedule.Id, ct);
            anySent = anySent || sent;
        }

        if (!string.IsNullOrWhiteSpace(customer.Email))
        {
            var (subject, htmlBody) = _composer.ComposeDebtReminderEmail(customer, customer.CurrentBalance, lastPaymentDate);
            var sent = await _sender.SendEmailAsync(customer.Email, subject, htmlBody, "debt_reminder", "CustomerDebtSchedule", schedule.Id, ct);
            anySent = anySent || sent;
        }

        return anySent;
    }
}
