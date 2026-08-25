// File: DTOs/Inventory/LowStockProductDto.cs
namespace IbnAlZumar.API.DTOs.Inventory;

/// <summary>
/// Flattened shape returned by GET /api/inventory/low-stock.
/// One row per product that is at or below its MinStockThreshold.
/// </summary>
public class LowStockProductDto
{
    public int Id { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }

    public int CurrentStock { get; set; }
    public int MinStockThreshold { get; set; }

    public decimal UnitPrice { get; set; }

    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    /// <summary>True when stock has hit zero (used to render the "نفذت الكمية" state vs. "اقترب من النفاد").</summary>
    public bool IsOutOfStock => CurrentStock <= 0;
}
