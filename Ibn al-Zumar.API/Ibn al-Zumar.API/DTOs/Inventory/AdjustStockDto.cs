using System.ComponentModel.DataAnnotations;

namespace IbnAlZumar.API.DTOs.Inventory
{
    /// <summary>
    /// Manual correction of ProductStock.QuantityOnHand for a single product at a single
    /// warehouse. QuantityChange is a signed delta: positive to add stock in (e.g. جرد سنوي
    /// found extra units), negative to remove it (e.g. تالف / هالك / خطأ إدخال).
    /// </summary>
    public class AdjustStockDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int WarehouseId { get; set; }

        [Required(ErrorMessage = "يجب إدخال قيمة التعديل")]
        public int QuantityChange { get; set; }

        /// <summary>One of: Damaged, Spoiled, StockCount, DataEntryError, Other.</summary>
        [Required, MaxLength(50)]
        public string Reason { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
