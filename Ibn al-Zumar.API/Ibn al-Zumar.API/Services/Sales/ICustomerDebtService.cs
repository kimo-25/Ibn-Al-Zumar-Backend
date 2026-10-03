using IbnAlZumar.API.DTOs.Common;
using IbnAlZumar.API.DTOs.Sales;

namespace IbnAlZumar.Api.Services.Sales;

public interface ICustomerDebtService
{
    Task<PagedResultDto<CustomerDebtDto>> GetDebtDashboardAsync(int pageNumber, int pageSize, CancellationToken ct = default);

    /// <summary>
    /// Creates/reactivates a CustomerDebtSchedule for every customer with CurrentBalance above
    /// DebtReminderOptions.MinimumBalanceToRemind that doesn't already have an active one, and
    /// deactivates schedules for customers who've since paid down to zero. Called by
    /// DebtReminderJob at the start of every run — no hook into Order/Payment creation needed,
    /// at the cost of up to one day of staleness (a schedule for a customer who just went into
    /// debt today is picked up by tomorrow's run, not instantly).
    /// </summary>
    Task ReconcileSchedulesAsync(CancellationToken ct = default);

    /// <summary>
    /// Sends every schedule whose NextReminderDueAt has passed, then advances it by
    /// DebtReminderOptions.IntervalDays. Called by DebtReminderJob. Returns how many were sent.
    /// </summary>
    Task<int> SendDueRemindersAsync(CancellationToken ct = default);

    /// <summary>Manual one-click "send now" from the dashboard, ignoring NextReminderDueAt.</summary>
    Task<CustomerDebtDto> SendReminderNowAsync(int customerId, CancellationToken ct = default);
}
