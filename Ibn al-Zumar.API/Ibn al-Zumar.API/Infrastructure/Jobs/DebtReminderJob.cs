using IbnAlZumar.Api.Services.Sales;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace IbnAlZumar.API.Infrastructure.Jobs;

/// <summary>
/// Per ARCHITECTURE.md §5.2b's job conventions: [DisallowConcurrentExecution], resolves a scoped
/// IServiceScope for the DbContext, logs start/finish, reads its cron from config section
/// "Quartz:Jobs:DebtReminderJob". Default cron here is daily at 10:00 Africa/Cairo
/// ("0 0 10 * * ?"), NOT the weekly-Sunday sketch in §5.2b — a daily run is what actually lets
/// CustomerDebtSchedule.NextReminderDueAt enforce a real "every 10 days, per customer" cadence
/// (a weekly-only job could only ever fire on 7-day boundaries). Idempotency comes from
/// NextReminderDueAt itself: a customer already reminded today won't be due again until
/// DebtReminderOptions.IntervalDays from now, so a re-run (or an early re-trigger) the same day
/// sends nothing further for them.
/// </summary>
[DisallowConcurrentExecution]
public class DebtReminderJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DebtReminderJob> _logger;

    public DebtReminderJob(IServiceScopeFactory scopeFactory, ILogger<DebtReminderJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("DebtReminderJob starting");

        using var scope = _scopeFactory.CreateScope();
        var debtService = scope.ServiceProvider.GetRequiredService<ICustomerDebtService>();

        try
        {
            await debtService.ReconcileSchedulesAsync(context.CancellationToken);
            var sentCount = await debtService.SendDueRemindersAsync(context.CancellationToken);

            _logger.LogInformation("DebtReminderJob finished — {SentCount} reminder(s) sent", sentCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DebtReminderJob failed");
            throw; // let Quartz record the failure; it will retry on the next scheduled fire
        }
    }
}
