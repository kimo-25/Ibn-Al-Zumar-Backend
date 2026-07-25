using IbnAlZumar.API.DTOs.Inventory;

namespace IbnAlZumar.API.Services.Inventory
{
    public class InventoryService : IInventoryService
    {
        public Task<StockTransactionResponseDto> AdjustStockAsync(AdjustStockDto dto)
        {
            // مجرد إرجاع نتيجة بدون تعديل قاعدة البيانات
            return Task.FromResult(new StockTransactionResponseDto
            {
                TransactionId = 1,
                ProductId = dto.ProductId,
                Quantity = dto.QuantityChange,
                TransactionType = "Adjust",
                TransactionDate = DateTime.UtcNow
            });
        }

        public Task<StockTransactionResponseDto> TransferStockAsync(TransferStockDto dto)
        {
            // مجرد إرجاع نتيجة بدون تعديل قاعدة البيانات
            return Task.FromResult(new StockTransactionResponseDto
            {
                TransactionId = 2,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                TransactionType = "Transfer",
                TransactionDate = DateTime.UtcNow
            });
        }
    }
}
