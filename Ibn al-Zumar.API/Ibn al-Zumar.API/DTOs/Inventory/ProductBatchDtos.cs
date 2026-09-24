using System.ComponentModel.DataAnnotations;

namespace IbnAlZumar.API.DTOs.Inventory;

/// <summary>Registers a new incoming batch and increases stock atomically (purchase receipt, opening balance, etc.).</summary>
public class ReceiveBatchDto
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    public int WarehouseId { get; set; }

    [Required, MaxLength(100)]
    public string BatchNumber { get; set; } = string.Empty;

    /// <summary>Optional — omit for legacy/opening-balance stock with no known supplier.</summary>
    public int? SupplierId { get; set; }

    public DateTime? ProductionDate { get; set; }

    [Required]
    public DateTime ExpiryDate { get; set; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "الكمية يجب أن تكون أكبر من صفر")]
    public int Quantity { get; set; }

    [Required, Range(0, double.MaxValue)]
    public decimal CostPrice { get; set; }

    /// <summary>"PurchaseReceived" (default) or "Adjustment" (e.g. opening balance / legacy stock entry).</summary>
    public string TransactionType { get; set; } = "PurchaseReceived";

    [MaxLength(50)]
    public string? ReferenceType { get; set; }
    public int? ReferenceId { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}

/// <summary>Consumes stock FEFO across whatever batches exist for the product/warehouse.</summary>
public class ConsumeFefoDto
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    public int WarehouseId { get; set; }

    [Required, Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    /// <summary>"Sale", "AdjustmentDecrease", "SupplierReturn", etc.</summary>
    public string TransactionType { get; set; } = "Sale";

    [MaxLength(50)]
    public string? ReferenceType { get; set; }
    public int? ReferenceId { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}

public class BatchConsumptionResultDto
{
    public int ProductBatchId { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public int QuantityConsumed { get; set; }
    public int RemainingQuantity { get; set; }
}

public class ProductBatchResponseDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductNameAr { get; set; }
    public string SKU { get; set; } = string.Empty;

    public string BatchNumber { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public int? SupplierId { get; set; }

    public DateTime? ProductionDate { get; set; }
    public DateTime ExpiryDate { get; set; }

    public int InitialQuantity { get; set; }
    public int RemainingQuantity { get; set; }
    public decimal CostPrice { get; set; }

    public bool IsDepleted { get; set; }
    public bool IsExpired { get; set; }
    public int DaysUntilExpiry { get; set; }
}

public class ExpiringBatchDto
{
    public int ProductBatchId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductNameAr { get; set; }
    public string SKU { get; set; } = string.Empty;

    public string BatchNumber { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;

    public DateTime ExpiryDate { get; set; }
    public int RemainingQuantity { get; set; }
    public int DaysUntilExpiry { get; set; }
    public bool IsExpired { get; set; }
}