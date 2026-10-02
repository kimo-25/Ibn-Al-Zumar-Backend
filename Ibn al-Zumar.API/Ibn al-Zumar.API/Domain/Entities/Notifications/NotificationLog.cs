using IbnAlZumar.Domain.Common;
using IbnAlZumar.Domain.Enums;

namespace IbnAlZumar.Domain.Entities.Notifications;

/// <summary>
/// Phase 3 — implements the NotificationLog described in ARCHITECTURE.md §5.2a. Every outbound
/// WhatsApp/Email/SMS send writes exactly one row here BEFORE the transport call and updates it
/// after — this is also the idempotency guard the Quartz jobs use (§5.2b: "guarded by a
/// NotificationLog... record so a re-run never double-notifies").
/// </summary>
public class NotificationLog : BaseEntity
{
    public NotificationChannel Channel { get; set; }

    /// <summary>Phone (E.164) for WhatsApp/SMS, email address for Email.</summary>
    public string Recipient { get; set; } = string.Empty;

    /// <summary>WhatsApp template name, or a short logical name for Email/SMS ("debt_reminder", "invoice_share").</summary>
    public string TemplateName { get; set; } = string.Empty;

    /// <summary>The composed template parameters / email body, as JSON — for audit + retry without re-composing.</summary>
    public string PayloadJson { get; set; } = string.Empty;

    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;

    /// <summary>Message id returned by the provider (Meta WhatsApp message id, Brevo message id).</summary>
    public string? ProviderMessageId { get; set; }

    public string? Error { get; set; }

    public DateTime? SentAtUtc { get; set; }

    // --- Additions beyond the architecture doc's field list, for the retry-with-backoff rule
    // (§5.2a) and for the debt-reminder job to know what it's retrying: ---

    public int RetryCount { get; set; } = 0;

    /// <summary>Loose polymorphic link back to what triggered this notification — "Order", "MaintenanceRequest", "CustomerDebtSchedule" — same pattern as InventoryTransaction.ReferenceType (§3.2).</summary>
    public string? RelatedEntityType { get; set; }
    public int? RelatedEntityId { get; set; }
}
