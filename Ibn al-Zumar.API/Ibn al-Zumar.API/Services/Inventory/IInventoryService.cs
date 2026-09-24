using IbnAlZumar.API.DTOs.Inventory;

namespace IbnAlZumar.API.Services.Inventory
{
    public interface IInventoryService
    {
        // --- Existing ---
        Task<StockTransactionResponseDto> AdjustStockAsync(AdjustStockDto dto);
        Task<StockTransferResponseDto> TransferStockAsync(TransferStockDto dto);
        Task<List<InventoryTransactionResponseDto>> GetTransactionHistoryAsync(int? productId, int? warehouseId, int take);
        Task<List<WarehouseDto>> GetWarehousesAsync();
        Task<List<StockLevelDto>> GetStockLevelsAsync(int? warehouseId, string? search);
        Task<List<StockLevelDto>> GetLowStockProductsAsync(int? warehouseId);

        // --- Sheet 1: 3-tier hierarchy ---
        /// <summary>Same rows as GetWarehousesAsync, but each carries Tier/ParentWarehouseId so the
        /// frontend can render Main -> Branch -> Shelf as a tree.</summary>
        Task<List<WarehouseDto>> GetWarehouseHierarchyAsync();

        // --- Sheet 1: batches & FEFO ---
        Task<ProductBatchResponseDto> ReceiveBatchAsync(ReceiveBatchDto dto);
        Task<List<BatchConsumptionResultDto>> ConsumeFefoAsync(ConsumeFefoDto dto);
        Task<List<ProductBatchResponseDto>> GetBatchesAsync(int? productId, int? warehouseId, bool includeDepleted);
        Task<List<ExpiringBatchDto>> GetExpiringBatchesAsync(int withinDays, int? warehouseId);
    }
}