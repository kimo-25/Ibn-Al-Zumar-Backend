using System.ComponentModel.DataAnnotations;

namespace IbnAlZumar.API.DTOs.Inventory
{
    /// <summary>
    /// Multi-item warehouse-to-warehouse transfer. Creates a StockTransfer header + one
    /// StockTransferItem per line, decrements ProductStock at the source, increments it at
    /// the destination, and writes a matching pair of InventoryTransaction rows (TransferOut /
    /// TransferIn) per item. Executes and completes synchronously (no separate "in-transit" step
    /// in Phase 1).
    /// </summary>
    public class TransferStockDto
    {
        [Required]
        public int FromWarehouseId { get; set; }

        [Required]
        public int ToWarehouseId { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [Required, MinLength(1, ErrorMessage = "يجب إضافة صنف واحد على الأقل للتحويل")]
        public List<TransferStockItemDto> Items { get; set; } = new();
    }

    public class TransferStockItemDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required, Range(1, int.MaxValue, ErrorMessage = "الكمية يجب أن تكون أكبر من صفر")]
        public int Quantity { get; set; }
    }
}
