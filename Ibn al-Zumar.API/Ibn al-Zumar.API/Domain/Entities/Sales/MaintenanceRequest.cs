using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
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

    [MaxLength(8000)]
    public string? ImageUrlsJson { get; set; }

    [NotMapped]
    public List<string> ImageUrls
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(ImageUrlsJson))
            {
                try { return JsonSerializer.Deserialize<List<string>>(ImageUrlsJson) ?? new List<string>(); }
                catch (JsonException) { }
            }
            return string.IsNullOrWhiteSpace(ImageUrl) ? new List<string>() : new List<string> { ImageUrl };
        }
        set => ImageUrlsJson = JsonSerializer.Serialize(value ?? new List<string>());
    }

    public DeliveryMethod DeliveryMethod { get; set; }
    public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Pending;

    public decimal? EstimatedPrice { get; set; }
    public DateTime? ScheduledDate { get; set; }

    [MaxLength(1000)]
    public string? AdminNotes { get; set; }

    [MaxLength(500)]
    public string? MaintenanceReportUrl { get; set; }
}