namespace IbnAlZumar.API.DTOs.Maintenance;

public class MaintenanceReceiptDto
{
    public int Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string ProblemDescription { get; set; } = string.Empty;
    public string StatusLabel { get; set; } = string.Empty;
    public decimal LaborCost { get; set; }
    public List<MaintenanceReceiptLineDto> Parts { get; set; } = new();
    public decimal PartsTotal { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
}

public class MaintenanceReceiptLineDto
{
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitCostPrice { get; set; }
    public decimal LineTotal { get; set; }
}
