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

        public string? Notes { get; set; }

        public List<CreatePurchaseOrderItemDto> Items { get; set; } = new();
    }

    public class CreatePurchaseOrderItemDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int QuantityOrdered { get; set; }

        [Required]
        public decimal UnitCostPrice { get; set; }
    }

    public class ApprovePurchaseOrderDto
    {
        public int PurchaseOrderId { get; set; }
        public DateTime ReceivedDate { get; set; } = DateTime.UtcNow;
    }

    public class PurchaseOrderResponseDto
    {
        public int Id { get; set; }
        public string PurchaseOrderNumber { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public int WarehouseId { get; set; }
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
        public int QuantityOrdered { get; set; }
        public int QuantityReceived { get; set; }
        public decimal UnitCostPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
