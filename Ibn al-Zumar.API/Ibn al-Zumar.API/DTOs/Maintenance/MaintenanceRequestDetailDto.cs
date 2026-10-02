using IbnAlZumar.Domain.Enums;

namespace IbnAlZumar.API.DTOs.Maintenance;

public class MaintenanceRequestListItemDto
{
    public int Id { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string ProblemDescription { get; set; } = string.Empty;
    public MaintenanceStatus Status { get; set; }
    public int? AssignedTechnicianUserId { get; set; }
    public string? AssignedTechnicianName { get; set; }
    public decimal? EstimatedPrice { get; set; }
    public decimal? ActualCost { get; set; }
    public int PartsCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ScheduledDate { get; set; }
}

public class MaintenanceNoteDto
{
    public int Id { get; set; }
    public int AuthorUserId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public MaintenanceStatus? StatusAtNote { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class MaintenancePartUsageDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductSku { get; set; }
    public int WarehouseId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitCostPrice { get; set; }
    public decimal LineTotal { get; set; }
}

public class MaintenanceRequestDetailDto
{
    public int Id { get; set; }
    public int? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string ProblemDescription { get; set; } = string.Empty;
    public List<string> ImageUrls { get; set; } = new();
    public int DeliveryMethod { get; set; }
    public MaintenanceStatus Status { get; set; }
    public decimal? EstimatedPrice { get; set; }
    public decimal LaborCost { get; set; }
    public decimal? ActualCost { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public string? AdminNotes { get; set; }
    public string? MaintenanceReportUrl { get; set; }
    public int? AssignedTechnicianUserId { get; set; }
    public string? AssignedTechnicianName { get; set; }
    public List<MaintenanceNoteDto> Notes { get; set; } = new();
    public List<MaintenancePartUsageDto> PartUsages { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

/// <summary>Lightweight option for the technician-assignment picker (GET /api/maintenance-workflow/technicians).</summary>
public class TechnicianOptionDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
}
