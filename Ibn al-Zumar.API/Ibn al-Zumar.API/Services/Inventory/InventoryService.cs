using IbnAlZumar.API.DTOs.Inventory;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.Domain.Entities.Inventory;
using IbnAlZumar.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace IbnAlZumar.API.Services.Inventory
{
    public class InventoryService : IInventoryService
    {
        private readonly ApplicationDbContext _context;

        public InventoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<StockTransactionResponseDto> AdjustStockAsync(AdjustStockDto dto)
        {
            if (dto.QuantityChange == 0)
                throw new InvalidOperationException("قيمة التعديل يجب ألا تساوي صفر");

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == dto.ProductId);
            if (product == null)
                throw new KeyNotFoundException("المنتج غير موجود");

            // لو الـ WarehouseId متبعتش من الفرونت أو قيمته 0، يتم التحويل تلقائياً على المخزن الرئيسي
            var warehouseId = dto.WarehouseId;
            if (warehouseId <= 0)
            {
                var mainWarehouse = await _context.Warehouses.FirstOrDefaultAsync(w => w.IsMainWarehouse && w.IsActive)
                                   ?? await _context.Warehouses.FirstOrDefaultAsync(w => w.IsActive);
                if (mainWarehouse == null)
                    throw new KeyNotFoundException("لا يوجد مستودع نشط في النظام");

                warehouseId = mainWarehouse.Id;
            }

            var warehouse = await _context.Warehouses.FirstOrDefaultAsync(w => w.Id == warehouseId);
            if (warehouse == null)
                throw new KeyNotFoundException("المستودع غير موجود");

            var stock = await _context.ProductStocks
                .FirstOrDefaultAsync(s => s.ProductId == dto.ProductId && s.WarehouseId == warehouseId);

            if (stock == null)
            {
                if (dto.QuantityChange < 0)
                    throw new InvalidOperationException("لا يمكن خصم كمية من مخزون غير موجود أصلاً لهذا المنتج في هذا المستودع");

                stock = new ProductStock
                {
                    ProductId = dto.ProductId,
                    WarehouseId = warehouseId,
                    QuantityOnHand = 0,
                    ReorderLevel = product.MinStockThreshold
                };
                _context.ProductStocks.Add(stock);
            }

            var newQuantity = stock.QuantityOnHand + dto.QuantityChange;
            if (newQuantity < 0)
                throw new InvalidOperationException($"الكمية الحالية ({stock.QuantityOnHand}) أقل من قيمة الخصم المطلوبة ({Math.Abs(dto.QuantityChange)})");

            stock.QuantityOnHand = newQuantity;
            if (dto.QuantityChange > 0)
                stock.LastRestockedAt = DateTime.UtcNow;

            var reasonLabel = TranslateAdjustReason(dto.Reason);
            var notes = string.IsNullOrWhiteSpace(dto.Notes) ? reasonLabel : $"{reasonLabel} — {dto.Notes}";

            var transactionEntity = new InventoryTransaction
            {
                ProductId = dto.ProductId,
                WarehouseId = warehouseId,
                TransactionType = InventoryTransactionType.Adjustment,
                QuantityChange = dto.QuantityChange,
                ReferenceType = "ManualAdjustment",
                ReferenceId = null,
                TransactionDate = DateTime.UtcNow,
                Notes = notes
            };
            _context.InventoryTransactions.Add(transactionEntity);

            await _context.SaveChangesAsync();

            return new StockTransactionResponseDto
            {
                TransactionId = transactionEntity.Id,
                ProductId = product.Id,
                ProductName = product.Name,
                WarehouseId = warehouse.Id,
                WarehouseName = warehouse.Name,
                QuantityChange = dto.QuantityChange,
                ResultingQuantityOnHand = stock.QuantityOnHand,
                TransactionType = transactionEntity.TransactionType.ToString(),
                Notes = transactionEntity.Notes,
                TransactionDate = transactionEntity.TransactionDate
            };
        }

        public async Task<StockTransferResponseDto> TransferStockAsync(TransferStockDto dto)
        {
            if (dto.FromWarehouseId == dto.ToWarehouseId)
                throw new InvalidOperationException("لا يمكن التحويل من وإلى نفس المستودع");

            var sourceWarehouse = await _context.Warehouses.FirstOrDefaultAsync(w => w.Id == dto.FromWarehouseId);
            if (sourceWarehouse == null)
                throw new KeyNotFoundException("المستودع المصدر غير موجود");

            var destWarehouse = await _context.Warehouses.FirstOrDefaultAsync(w => w.Id == dto.ToWarehouseId);
            if (destWarehouse == null)
                throw new KeyNotFoundException("المستودع المستقبل غير موجود");

            await using var dbTransaction = await _context.Database.BeginTransactionAsync();

            var transfer = new StockTransfer
            {
                SourceWarehouseId = dto.FromWarehouseId,
                DestinationWarehouseId = dto.ToWarehouseId,
                Status = StockTransferStatus.Requested,
                RequestedAt = DateTime.UtcNow,
                Notes = dto.Notes,
                Items = new List<StockTransferItem>()
            };
            _context.StockTransfers.Add(transfer);

            var responseItems = new List<StockTransferItemResponseDto>();

            foreach (var item in dto.Items)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);
                if (product == null)
                    throw new KeyNotFoundException($"المنتج رقم {item.ProductId} غير موجود");

                var sourceStock = await _context.ProductStocks
                    .FirstOrDefaultAsync(s => s.ProductId == item.ProductId && s.WarehouseId == dto.FromWarehouseId);

                if (sourceStock == null || sourceStock.QuantityOnHand < item.Quantity)
                {
                    var available = sourceStock?.QuantityOnHand ?? 0;
                    throw new InvalidOperationException(
                        $"الكمية المتاحة من \"{product.Name}\" في المستودع المصدر ({available}) أقل من الكمية المطلوب تحويلها ({item.Quantity})");
                }

                var destStock = await _context.ProductStocks
                    .FirstOrDefaultAsync(s => s.ProductId == item.ProductId && s.WarehouseId == dto.ToWarehouseId);

                if (destStock == null)
                {
                    destStock = new ProductStock
                    {
                        ProductId = item.ProductId,
                        WarehouseId = dto.ToWarehouseId,
                        QuantityOnHand = 0,
                        ReorderLevel = product.MinStockThreshold
                    };
                    _context.ProductStocks.Add(destStock);
                }

                sourceStock.QuantityOnHand -= item.Quantity;
                destStock.QuantityOnHand += item.Quantity;
                destStock.LastRestockedAt = DateTime.UtcNow;

                transfer.Items.Add(new StockTransferItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });

                responseItems.Add(new StockTransferItemResponseDto
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    SKU = product.SKU,
                    Quantity = item.Quantity
                });
            }

            transfer.Status = StockTransferStatus.Completed;
            transfer.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            foreach (var item in dto.Items)
            {
                _context.InventoryTransactions.Add(new InventoryTransaction
                {
                    ProductId = item.ProductId,
                    WarehouseId = dto.FromWarehouseId,
                    TransactionType = InventoryTransactionType.TransferOut,
                    QuantityChange = -item.Quantity,
                    ReferenceType = "StockTransfer",
                    ReferenceId = transfer.Id,
                    TransactionDate = transfer.CompletedAt.Value,
                    Notes = $"تحويل إلى {destWarehouse.Name}" + (string.IsNullOrWhiteSpace(dto.Notes) ? "" : $" — {dto.Notes}")
                });

                _context.InventoryTransactions.Add(new InventoryTransaction
                {
                    ProductId = item.ProductId,
                    WarehouseId = dto.ToWarehouseId,
                    TransactionType = InventoryTransactionType.TransferIn,
                    QuantityChange = item.Quantity,
                    ReferenceType = "StockTransfer",
                    ReferenceId = transfer.Id,
                    TransactionDate = transfer.CompletedAt.Value,
                    Notes = $"تحويل من {sourceWarehouse.Name}" + (string.IsNullOrWhiteSpace(dto.Notes) ? "" : $" — {dto.Notes}")
                });
            }

            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync();

            return new StockTransferResponseDto
            {
                StockTransferId = transfer.Id,
                SourceWarehouseId = sourceWarehouse.Id,
                SourceWarehouseName = sourceWarehouse.Name,
                DestinationWarehouseId = destWarehouse.Id,
                DestinationWarehouseName = destWarehouse.Name,
                Status = transfer.Status.ToString(),
                RequestedAt = transfer.RequestedAt,
                CompletedAt = transfer.CompletedAt,
                Notes = transfer.Notes,
                Items = responseItems
            };
        }

        public async Task<List<InventoryTransactionResponseDto>> GetTransactionHistoryAsync(int? productId, int? warehouseId, int take)
        {
            var query = _context.InventoryTransactions
                .AsNoTracking()
                .Include(t => t.Product)
                .Include(t => t.Warehouse)
                .AsQueryable();

            if (productId.HasValue)
                query = query.Where(t => t.ProductId == productId.Value);

            if (warehouseId.HasValue)
                query = query.Where(t => t.WarehouseId == warehouseId.Value);

            return await query
                .OrderByDescending(t => t.TransactionDate)
                .Take(take <= 0 ? 100 : take)
                .Select(t => new InventoryTransactionResponseDto
                {
                    Id = t.Id,
                    ProductId = t.ProductId,
                    ProductName = t.Product.Name,
                    ProductNameAr = t.Product.NameAr,
                    SKU = t.Product.SKU,
                    WarehouseId = t.WarehouseId,
                    WarehouseName = t.Warehouse.Name,
                    TransactionType = t.TransactionType.ToString(),
                    QuantityChange = t.QuantityChange,
                    ReferenceType = t.ReferenceType,
                    ReferenceId = t.ReferenceId,
                    Notes = t.Notes,
                    TransactionDate = t.TransactionDate
                })
                .ToListAsync();
        }

        public async Task<List<WarehouseDto>> GetWarehousesAsync()
        {
            return await _context.Warehouses
                .AsNoTracking()
                .Where(w => w.IsActive)
                .OrderByDescending(w => w.IsMainWarehouse)
                .ThenBy(w => w.Name)
                .Select(w => new WarehouseDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    IsMainWarehouse = w.IsMainWarehouse,
                    IsActive = w.IsActive
                })
                .ToListAsync();
        }

        public async Task<List<StockLevelDto>> GetStockLevelsAsync(int? warehouseId, string? search)
        {
            var query = _context.ProductStocks
                .AsNoTracking()
                .Include(s => s.Product)
                .Where(s => s.Product.IsActive)
                .AsQueryable();

            if (warehouseId.HasValue)
                query = query.Where(s => s.WarehouseId == warehouseId.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(s =>
                    s.Product.Name.Contains(term) ||
                    (s.Product.NameAr != null && s.Product.NameAr.Contains(term)) ||
                    s.Product.SKU.Contains(term));
            }

            return await query
                .OrderBy(s => s.Product.Name)
                .Select(s => new StockLevelDto
                {
                    ProductId = s.ProductId,
                    SKU = s.Product.SKU,
                    ProductName = s.Product.Name,
                    ProductNameAr = s.Product.NameAr,
                    ImageUrl = s.Product.ImageUrl,
                    WarehouseId = s.WarehouseId,
                    QuantityOnHand = s.QuantityOnHand,
                    ReorderLevel = s.ReorderLevel
                })
                .ToListAsync();
        }

        private static string TranslateAdjustReason(string reason) => reason switch
        {
            "Damaged" => "تالف",
            "Spoiled" => "هالك",
            "StockCount" => "جرد سنوي",
            "DataEntryError" => "خطأ إدخال",
            "Other" => "أخرى",
            _ => reason
        };
    }
}