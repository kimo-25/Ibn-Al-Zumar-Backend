using IbnAlZumar.API.DTOs.Inventory;

namespace IbnAlZumar.API.Services.Inventory
{
    public interface IInventoryService
    {
        Task<StockTransactionResponseDto> AdjustStockAsync(AdjustStockDto dto);
        Task<StockTransactionResponseDto> TransferStockAsync(TransferStockDto dto);
    }
}
