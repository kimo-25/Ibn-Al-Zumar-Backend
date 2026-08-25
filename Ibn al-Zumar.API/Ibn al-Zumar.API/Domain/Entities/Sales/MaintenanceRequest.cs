using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IbnAlZumar.Domain.Common;
using IbnAlZumar.Domain.Entities.Identity;
using IbnAlZumar.Domain.Entities.Sales;
using IbnAlZumar.Domain.Enums;

namespace IbnAlZumar.Domain.Entities.Maintenance;

public class MaintenanceRequest : BaseEntity
{
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public int? UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    [Required, MaxLength(1000)]
    public string ProblemDescription { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public DeliveryMethod DeliveryMethod { get; set; }
    public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Pending;

    public decimal? EstimatedPrice { get; set; }
    public DateTime? ScheduledDate { get; set; }

    [MaxLength(1000)]
    public string? AdminNotes { get; set; }

    [MaxLength(500)]
    public string? MaintenanceReportUrl { get; set; }
}