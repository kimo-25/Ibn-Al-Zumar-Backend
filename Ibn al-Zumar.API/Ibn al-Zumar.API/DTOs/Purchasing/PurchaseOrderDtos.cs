using System.ComponentModel.DataAnnotations;

namespace IbnAlZumar.API.DTOs.Purchasing
{
    public class CreatePurchaseOrderDto
    {
        [Required, MaxLength(50)]
        public string PurchaseOrderNumber { get; set; } = string.Empty;

        [Required]
        public int SupplierId { get; set; }

        [Required]
        public int WarehouseId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public DateTime? ExpectedDeliveryDate { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [Required, MinLength(1, ErrorMessage = "يجب إضافة صنف واحد على الأقل لأمر الشراء")]
        public List<CreatePurchaseOrderItemDto> Items { get; set; } = new();
    }

    public class CreatePurchaseOrderItemDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required, Range(1, int.MaxValue, ErrorMessage = "الكمية يجب أن تكون أكبر من صفر")]
        public int QuantityOrdered { get; set; }

        [Required, Range(0.01, double.MaxValue, ErrorMessage = "سعر التكلفة يجب أن يكون أكبر من صفر")]
        public decimal UnitCostPrice { get; set; }
    }

    /// <summary>
    /// Marks a Draft purchase order as Received: increases ProductStock.QuantityOnHand,
    /// writes InventoryTransaction rows, updates Product.CurrentCostPrice and
    /// ProductStock.LastRestockedAt, and increases Supplier.CurrentBalance by the order total.
    /// </summary>
    public class ApprovePurchaseOrderDto
    {
        [Required]
        public int PurchaseOrderId { get; set; }
        public DateTime ReceivedDate { get; set; } = DateTime.UtcNow;
    }

    public class PurchaseOrderResponseDto
    {
        public int Id { get; set; }
        public string PurchaseOrderNumber { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public decimal TotalCost { get; set; }
        public string? Notes { get; set; }
        public List<PurchaseOrderItemResponseDto> Items { get; set; } = new();
    }

    public class PurchaseOrderItemResponseDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductNameAr { get; set; }
        public string SKU { get; set; } = string.Empty;
        public int QuantityOrdered { get; set; }
        public int QuantityReceived { get; set; }
        public decimal UnitCostPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
