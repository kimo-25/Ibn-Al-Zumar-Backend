using IbnAlZumar.Domain.Entities.Maintenance;
using IbnAlZumar.Domain.Common;
using IbnAlZumar.Domain.Entities.Identity;
using IbnAlZumar.Domain.Enums;

namespace IbnAlZumar.Domain.Entities.Maintenance;

/// <summary>
/// Phase 2 — a running, timestamped log of technician/admin notes on a MaintenanceRequest.
/// Distinct from the existing MaintenanceRequest.AdminNotes (a single free-text field, kept
/// as-is for backward compatibility with the online-inquiry pricing screen). Also doubles as
/// the audit trail for status transitions: MaintenanceWorkflowService writes one of these on
/// every ChangeStatusAsync call, with StatusAtNote set to the new status.
/// </summary>
public class MaintenanceNote : BaseEntity
{
    public int MaintenanceRequestId { get; set; }
    public MaintenanceRequest MaintenanceRequest { get; set; } = null!;

    public int AuthorUserId { get; set; }
    public User AuthorUser { get; set; } = null!;

    public string Note { get; set; } = string.Empty;

    /// <summary>The status the ticket was moved to at the same time this note was written, if any.</summary>
    public MaintenanceStatus? StatusAtNote { get; set; }
}
