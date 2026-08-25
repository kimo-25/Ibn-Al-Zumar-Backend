namespace IbnAlZumar.API.DTOs.Inventory
{
    /// <summary>Powers the product picker in the Adjust / Transfer forms — shows live QOH per warehouse.</summary>
    public class StockLevelDto
    {
        public int ProductId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? ProductNameAr { get; set; }
        public string? ImageUrl { get; set; }
        public int WarehouseId { get; set; }
        public int QuantityOnHand { get; set; }
        public int ReorderLevel { get; set; }
    }
}
