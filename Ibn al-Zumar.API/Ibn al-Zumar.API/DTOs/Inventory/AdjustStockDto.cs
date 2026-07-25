namespace IbnAlZumar.API.DTOs.Inventory
{
    public class AdjustStockDto
    {
        public int ProductId { get; set; }
        public int QuantityChange { get; set; }
        public string? Reason { get; set; }
    }
}
