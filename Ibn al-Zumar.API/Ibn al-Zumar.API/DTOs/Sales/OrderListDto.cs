using System;

namespace IbnAlZumar.API.DTOs.Sales;

public class OrderListDto
{
    public int Id { get; set; }
    public string? OrderNumber { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }

    // Customer / guest summary
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;

    public int ItemsCount { get; set; }
}