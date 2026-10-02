namespace IbnAlZumar.API.Common.Settings;

/// <summary>Bound from appsettings config section "DebtReminder". See BACKEND_CHANGES_PHASE3.md §1.</summary>
public class DebtReminderOptions
{
    /// <summary>
    /// How often (in days) a customer with an outstanding balance gets re-reminded, for as long
    /// as they still owe money. Default 10, per the original Phase 3 requirement ("تنبيه دورية
    /// كل 10 أيام") — intentionally different from the 14-day one-shot idle default sketched for
    /// DebtReminderJob in ARCHITECTURE.md §5.2b.
    /// </summary>
    public int IntervalDays { get; set; } = 10;

    /// <summary>Customers below this balance are never scheduled — avoids nagging over a rounding-error few piastres.</summary>
    public decimal MinimumBalanceToRemind { get; set; } = 1m;
}
