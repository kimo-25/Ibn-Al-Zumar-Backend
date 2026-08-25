namespace IbnAlZumar.API.DTOs.Inventory
{
    /// <summary>Row shape for the audit-history table (GET /api/inventory/transactions).</summary>
    public class InventoryTransactionResponseDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductNameAr { get; set; }
        public string SKU { get; set; } = string.Empty;
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public int QuantityChange { get; set; }
        public string? ReferenceType { get; set; }
        public int? ReferenceId { get; set; }
        public string? Notes { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
