namespace IbnAlZumar.API.DTOs.Inventory
{
    public class StockTransactionResponseDto
    {
        public int TransactionId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
    }
}
