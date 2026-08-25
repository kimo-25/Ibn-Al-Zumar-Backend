namespace IbnAlZumar.API.DTOs.Inventory
{
    /// <summary>Returned after a single-item operation (adjustment).</summary>
    public class StockTransactionResponseDto
    {
        public int TransactionId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public int QuantityChange { get; set; }
        public int ResultingQuantityOnHand { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
