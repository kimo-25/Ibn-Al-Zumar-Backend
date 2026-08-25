namespace IbnAlZumar.API.DTOs.Inventory
{
    /// <summary>Returned after a (possibly multi-item) transfer between two warehouses.</summary>
    public class StockTransferResponseDto
    {
        public int StockTransferId { get; set; }
        public int SourceWarehouseId { get; set; }
        public string SourceWarehouseName { get; set; } = string.Empty;
        public int DestinationWarehouseId { get; set; }
        public string DestinationWarehouseName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Notes { get; set; }
        public List<StockTransferItemResponseDto> Items { get; set; } = new();
    }

    public class StockTransferItemResponseDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
