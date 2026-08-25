using IbnAlZumar.API.DTOs.Inventory;

namespace IbnAlZumar.API.Services.Inventory
{
    public interface IInventoryService
    {
        Task<StockTransactionResponseDto> AdjustStockAsync(AdjustStockDto dto);
        Task<StockTransferResponseDto> TransferStockAsync(TransferStockDto dto);
        Task<List<InventoryTransactionResponseDto>> GetTransactionHistoryAsync(int? productId, int? warehouseId, int take);
        Task<List<WarehouseDto>> GetWarehousesAsync();
        Task<List<StockLevelDto>> GetStockLevelsAsync(int? warehouseId, string? search);
    }
}
