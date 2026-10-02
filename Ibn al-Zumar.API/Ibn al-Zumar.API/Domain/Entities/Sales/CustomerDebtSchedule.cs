using IbnAlZumar.Domain.Common;
using IbnAlZumar.Domain.Entities.Sales;
using System.ComponentModel.DataAnnotations.Schema;

namespace IbnAlZumar.Domain.Entities.Sales;

/// <summary>
/// Phase 3 — one row per customer who currently owes the store money (Customer.CurrentBalance
/// &gt; 0). Tracks a RECURRING reminder cadence (every DebtReminderOptions.IntervalDays, default
/// 10 — see BACKEND_CHANGES_PHASE3.md §1) rather than the single idle-triggered ping described
/// for the simpler DebtReminderJob sketch in ARCHITECTURE.md §5.2b: as long as the customer still
/// owes money, DebtReminderJob keeps nudging NextReminderDueAt forward by IntervalDays every time
/// it actually sends, so the customer gets a fresh reminder every N days until they pay — not
/// just once.
///
/// CustomerDebtService keeps this table in sync: a schedule is created/reactivated the moment
/// CustomerLedgerEntry pushes Customer.CurrentBalance above zero, and deactivated (IsActive =
/// false) the moment a payment brings it back to zero or below. CustomerLedgerEntry.RunningBalance
/// remains the single source of debt-amount truth (§7.10) — LastKnownBalance here is only a
/// display snapshot for composing the reminder message, refreshed on every send.
/// </summary>
public class CustomerDebtSchedule : BaseEntity
{
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    [Column(TypeName = "decimal(18,2)")]
    public decimal LastKnownBalance { get; set; }

    public DateTime? LastReminderSentAt { get; set; }

    public DateTime NextReminderDueAt { get; set; } = DateTime.UtcNow;

    public int ReminderCount { get; set; } = 0;
}
