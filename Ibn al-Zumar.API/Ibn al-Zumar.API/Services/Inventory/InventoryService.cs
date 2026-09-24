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

            // Sheet 1: if this product has batches in this warehouse and we're deducting stock,
            // consume via FEFO so RemainingQuantity per batch stays accurate. Positive adjustments
            // (restocks without a known batch/expiry) still go through the simple path below —
            // use ReceiveBatchAsync when the batch/expiry is known.
            if (dto.QuantityChange < 0)
            {
                var hasBatches = await _context.Set<ProductBatch>()
                    .AnyAsync(b => b.ProductId == dto.ProductId && b.WarehouseId == warehouseId && b.RemainingQuantity > 0);

                if (hasBatches)
                {
                    var reasonLabelForFefo = TranslateAdjustReason(dto.Reason);
                    var notesForFefo = string.IsNullOrWhiteSpace(dto.Notes) ? reasonLabelForFefo : $"{reasonLabelForFefo} — {dto.Notes}";

                    await ConsumeFefoAsync(new ConsumeFefoDto
                    {
                        ProductId = dto.ProductId,
                        WarehouseId = warehouseId,
                        Quantity = Math.Abs(dto.QuantityChange),
                        TransactionType = nameof(InventoryTransactionType.AdjustmentDecrease),
                        ReferenceType = "ManualAdjustment",
                        ReferenceId = null,
                        Notes = notesForFefo
                    });

                    var updatedStock = await _context.ProductStocks
                        .FirstAsync(s => s.ProductId == dto.ProductId && s.WarehouseId == warehouseId);

                    return new StockTransactionResponseDto
                    {
                        TransactionId = 0, // multiple transaction rows were written (one per batch consumed) — see GetTransactionHistoryAsync
                        ProductId = product.Id,
                        ProductName = product.Name,
                        WarehouseId = warehouse.Id,
                        WarehouseName = warehouse.Name,
                        QuantityChange = dto.QuantityChange,
                        ResultingQuantityOnHand = updatedStock.QuantityOnHand,
                        TransactionType = InventoryTransactionType.AdjustmentDecrease.ToString(),
                        Notes = notesForFefo,
                        TransactionDate = DateTime.UtcNow
                    };
                }
            }

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

            // Sheet 1: transfers must follow the declared hierarchy — one side must be the direct
            // parent of the other (Main -> Branch, Branch -> Shelf, or the reverse for returns).
            ValidateWarehouseHierarchy(sourceWarehouse, destWarehouse);

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
                .ThenBy(w => w.Tier)
                .ThenBy(w => w.Name)
                .Select(w => new WarehouseDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    IsMainWarehouse = w.IsMainWarehouse,
                    IsActive = w.IsActive,
                    Tier = w.Tier,
                    TierName = w.Tier.ToString(),
                    ParentWarehouseId = w.ParentWarehouseId,
                    ParentWarehouseName = w.ParentWarehouse != null ? w.ParentWarehouse.Name : null
                })
                .ToListAsync();
        }

        public async Task<List<WarehouseDto>> GetWarehouseHierarchyAsync()
        {
            // Same shape as GetWarehousesAsync — the frontend builds the Main->Branch->Shelf tree
            // client-side from Tier + ParentWarehouseId, so we just guarantee a parent-first ordering.
            return await _context.Warehouses
                .AsNoTracking()
                .Where(w => w.IsActive)
                .OrderBy(w => w.Tier)
                .ThenBy(w => w.ParentWarehouseId ?? 0)
                .ThenBy(w => w.Name)
                .Select(w => new WarehouseDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    IsMainWarehouse = w.IsMainWarehouse,
                    IsActive = w.IsActive,
                    Tier = w.Tier,
                    TierName = w.Tier.ToString(),
                    ParentWarehouseId = w.ParentWarehouseId,
                    ParentWarehouseName = w.ParentWarehouse != null ? w.ParentWarehouse.Name : null
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

        public async Task<List<StockLevelDto>> GetLowStockProductsAsync(int? warehouseId)
        {
            var query = _context.Products
                .AsNoTracking()
                .Where(p => p.IsActive)
                .AsQueryable();

            return await query
                .Select(p => new
                {
                    Product = p,
                    TotalStock = p.Stocks
                        .Where(s => !warehouseId.HasValue || s.WarehouseId == warehouseId.Value)
                        .Sum(s => (int?)s.QuantityOnHand) ?? 0
                })
                .Where(x => x.TotalStock <= 0 || x.TotalStock <= x.Product.MinStockThreshold)
                .OrderBy(x => x.TotalStock)
                .Select(x => new StockLevelDto
                {
                    ProductId = x.Product.Id,
                    SKU = x.Product.SKU,
                    ProductName = x.Product.Name,
                    ProductNameAr = x.Product.NameAr,
                    ImageUrl = x.Product.ImageUrl,
                    WarehouseId = warehouseId ?? 0,
                    QuantityOnHand = x.TotalStock,
                    ReorderLevel = x.Product.MinStockThreshold
                })
                .ToListAsync();
        }

        // ================= Sheet 1: Product Batches & FEFO =================

        public async Task<ProductBatchResponseDto> ReceiveBatchAsync(ReceiveBatchDto dto)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == dto.ProductId);
            if (product == null)
                throw new KeyNotFoundException("المنتج غير موجود");

            var warehouse = await _context.Warehouses.FirstOrDefaultAsync(w => w.Id == dto.WarehouseId);
            if (warehouse == null)
                throw new KeyNotFoundException("المستودع غير موجود");

            if (dto.ProductionDate.HasValue && dto.ProductionDate.Value.Date > dto.ExpiryDate.Date)
                throw new InvalidOperationException("تاريخ الإنتاج لا يمكن أن يكون بعد تاريخ انتهاء الصلاحية");

            if (!Enum.TryParse<InventoryTransactionType>(dto.TransactionType, ignoreCase: true, out var transactionType))
                transactionType = InventoryTransactionType.PurchaseReceived;

            await using var dbTransaction = await _context.Database.BeginTransactionAsync();

            var batch = new ProductBatch
            {
                ProductId = dto.ProductId,
                BatchNumber = dto.BatchNumber,
                WarehouseId = dto.WarehouseId,
                SupplierId = dto.SupplierId,
                ProductionDate = dto.ProductionDate,
                ExpiryDate = dto.ExpiryDate,
                InitialQuantity = dto.Quantity,
                RemainingQuantity = dto.Quantity,
                CostPrice = dto.CostPrice
            };
            _context.Set<ProductBatch>().Add(batch);

            var stock = await _context.ProductStocks
                .FirstOrDefaultAsync(s => s.ProductId == dto.ProductId && s.WarehouseId == dto.WarehouseId);

            if (stock == null)
            {
                stock = new ProductStock
                {
                    ProductId = dto.ProductId,
                    WarehouseId = dto.WarehouseId,
                    QuantityOnHand = 0,
                    ReorderLevel = product.MinStockThreshold
                };
                _context.ProductStocks.Add(stock);
            }

            stock.QuantityOnHand += dto.Quantity;
            stock.LastRestockedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(); // assigns batch.Id

            _context.InventoryTransactions.Add(new InventoryTransaction
            {
                ProductId = dto.ProductId,
                WarehouseId = dto.WarehouseId,
                TransactionType = transactionType,
                QuantityChange = dto.Quantity,
                ProductBatchId = batch.Id,
                ReferenceType = dto.ReferenceType,
                ReferenceId = dto.ReferenceId,
                TransactionDate = DateTime.UtcNow,
                Notes = dto.Notes
            });

            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync();

            return new ProductBatchResponseDto
            {
                Id = batch.Id,
                ProductId = product.Id,
                ProductName = product.Name,
                ProductNameAr = product.NameAr,
                SKU = product.SKU,
                BatchNumber = batch.BatchNumber,
                WarehouseId = warehouse.Id,
                WarehouseName = warehouse.Name,
                SupplierId = batch.SupplierId,
                ProductionDate = batch.ProductionDate,
                ExpiryDate = batch.ExpiryDate,
                InitialQuantity = batch.InitialQuantity,
                RemainingQuantity = batch.RemainingQuantity,
                CostPrice = batch.CostPrice,
                IsDepleted = batch.IsDepleted,
                IsExpired = batch.IsExpired,
                DaysUntilExpiry = (batch.ExpiryDate.Date - DateTime.UtcNow.Date).Days
            };
        }

        public async Task<List<BatchConsumptionResultDto>> ConsumeFefoAsync(ConsumeFefoDto dto)
        {
            if (dto.Quantity <= 0)
                throw new InvalidOperationException("الكمية المطلوب صرفها يجب أن تكون أكبر من صفر");

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == dto.ProductId);
            if (product == null)
                throw new KeyNotFoundException("المنتج غير موجود");

            var warehouse = await _context.Warehouses.FirstOrDefaultAsync(w => w.Id == dto.WarehouseId);
            if (warehouse == null)
                throw new KeyNotFoundException("المستودع غير موجود");

            if (!Enum.TryParse<InventoryTransactionType>(dto.TransactionType, ignoreCase: true, out var transactionType))
                transactionType = InventoryTransactionType.Sale;

            // First-Expired-First-Out: oldest expiry first. Ties broken by CreatedAt (oldest batch first).
            var batches = await _context.Set<ProductBatch>()
                .Where(b => b.ProductId == dto.ProductId && b.WarehouseId == dto.WarehouseId && b.RemainingQuantity > 0)
                .OrderBy(b => b.ExpiryDate)
                .ThenBy(b => b.CreatedAt)
                .ToListAsync();

            var totalAvailable = batches.Sum(b => b.RemainingQuantity);
            if (totalAvailable < dto.Quantity)
                throw new InvalidOperationException(
                    $"الكمية المتاحة في الدفعات ({totalAvailable}) أقل من الكمية المطلوب صرفها ({dto.Quantity}) لمنتج \"{product.Name}\"");

            var stock = await _context.ProductStocks
                .FirstOrDefaultAsync(s => s.ProductId == dto.ProductId && s.WarehouseId == dto.WarehouseId);
            if (stock == null || stock.QuantityOnHand < dto.Quantity)
                throw new InvalidOperationException("رصيد المخزون الإجمالي لا يتطابق مع أرصدة الدفعات — يرجى مراجعة الجرد");

            await using var dbTransaction = await _context.Database.BeginTransactionAsync();

            var remainingToConsume = dto.Quantity;
            var results = new List<BatchConsumptionResultDto>();

            foreach (var batch in batches)
            {
                if (remainingToConsume <= 0) break;

                var consumeFromThisBatch = Math.Min(batch.RemainingQuantity, remainingToConsume);
                batch.RemainingQuantity -= consumeFromThisBatch;
                remainingToConsume -= consumeFromThisBatch;

                _context.InventoryTransactions.Add(new InventoryTransaction
                {
                    ProductId = dto.ProductId,
                    WarehouseId = dto.WarehouseId,
                    TransactionType = transactionType,
                    QuantityChange = -consumeFromThisBatch,
                    ProductBatchId = batch.Id,
                    ReferenceType = dto.ReferenceType,
                    ReferenceId = dto.ReferenceId,
                    TransactionDate = DateTime.UtcNow,
                    Notes = dto.Notes
                });

                results.Add(new BatchConsumptionResultDto
                {
                    ProductBatchId = batch.Id,
                    BatchNumber = batch.BatchNumber,
                    ExpiryDate = batch.ExpiryDate,
                    QuantityConsumed = consumeFromThisBatch,
                    RemainingQuantity = batch.RemainingQuantity
                });
            }

            stock.QuantityOnHand -= dto.Quantity;

            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync();

            return results;
        }

        public async Task<List<ProductBatchResponseDto>> GetBatchesAsync(int? productId, int? warehouseId, bool includeDepleted)
        {
            var query = _context.Set<ProductBatch>()
                .AsNoTracking()
                .Include(b => b.Product)
                .Include(b => b.Warehouse)
                .AsQueryable();

            if (productId.HasValue)
                query = query.Where(b => b.ProductId == productId.Value);

            if (warehouseId.HasValue)
                query = query.Where(b => b.WarehouseId == warehouseId.Value);

            if (!includeDepleted)
                query = query.Where(b => b.RemainingQuantity > 0);

            var today = DateTime.UtcNow.Date;

            return await query
                .OrderBy(b => b.ExpiryDate)
                .Select(b => new ProductBatchResponseDto
                {
                    Id = b.Id,
                    ProductId = b.ProductId,
                    ProductName = b.Product.Name,
                    ProductNameAr = b.Product.NameAr,
                    SKU = b.Product.SKU,
                    BatchNumber = b.BatchNumber,
                    WarehouseId = b.WarehouseId,
                    WarehouseName = b.Warehouse.Name,
                    SupplierId = b.SupplierId,
                    ProductionDate = b.ProductionDate,
                    ExpiryDate = b.ExpiryDate,
                    InitialQuantity = b.InitialQuantity,
                    RemainingQuantity = b.RemainingQuantity,
                    CostPrice = b.CostPrice,
                    IsDepleted = b.RemainingQuantity <= 0,
                    IsExpired = b.ExpiryDate.Date < today,
                    DaysUntilExpiry = EF.Functions.DateDiffDay(today, b.ExpiryDate)
                })
                .ToListAsync();
        }

        public async Task<List<ExpiringBatchDto>> GetExpiringBatchesAsync(int withinDays, int? warehouseId)
        {
            if (withinDays <= 0) withinDays = 30; // matches config ExpiryWarningDays default (§5.2 ProductExpiryAlertJob)

            var today = DateTime.UtcNow.Date;
            var threshold = today.AddDays(withinDays);

            var query = _context.Set<ProductBatch>()
                .AsNoTracking()
                .Include(b => b.Product)
                .Include(b => b.Warehouse)
                .Where(b => b.RemainingQuantity > 0 && b.ExpiryDate.Date <= threshold);

            if (warehouseId.HasValue)
                query = query.Where(b => b.WarehouseId == warehouseId.Value);

            return await query
                .OrderBy(b => b.ExpiryDate)
                .Select(b => new ExpiringBatchDto
                {
                    ProductBatchId = b.Id,
                    ProductId = b.ProductId,
                    ProductName = b.Product.Name,
                    ProductNameAr = b.Product.NameAr,
                    SKU = b.Product.SKU,
                    BatchNumber = b.BatchNumber,
                    WarehouseId = b.WarehouseId,
                    WarehouseName = b.Warehouse.Name,
                    ExpiryDate = b.ExpiryDate,
                    RemainingQuantity = b.RemainingQuantity,
                    DaysUntilExpiry = EF.Functions.DateDiffDay(today, b.ExpiryDate),
                    IsExpired = b.ExpiryDate.Date < today
                })
                .ToListAsync();
        }

        // ================= Sheet 1: hierarchy validation =================

        /// <summary>
        /// A transfer is only valid along an adjacent hierarchy edge: one warehouse must be the
        /// direct ParentWarehouseId of the other (either direction — down for distribution,
        /// up for returns). Sibling-to-sibling or skip-level transfers (e.g. Main -> Shelf when a
        /// Branch sits between them) are rejected so stock always flows through the declared chain.
        /// </summary>
        private static void ValidateWarehouseHierarchy(Warehouse source, Warehouse destination)
        {
            var destIsChildOfSource = destination.ParentWarehouseId == source.Id;
            var sourceIsChildOfDest = source.ParentWarehouseId == destination.Id;

            if (destIsChildOfSource || sourceIsChildOfDest)
                return;

            // Two top-level MainCentral warehouses transferring directly to each other is allowed
            // (e.g. inter-branch-of-last-resort at the top tier); anything else is rejected.
            if (source.Tier == WarehouseTier.MainCentral && destination.Tier == WarehouseTier.MainCentral)
                return;

            throw new InvalidOperationException(
                $"لا يمكن التحويل مباشرة بين \"{source.Name}\" و\"{destination.Name}\" — التحويل مسموح فقط بين مستودع وفرعه المباشر في التسلسل الهرمي (رئيسي ← فرع ← رف)");
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