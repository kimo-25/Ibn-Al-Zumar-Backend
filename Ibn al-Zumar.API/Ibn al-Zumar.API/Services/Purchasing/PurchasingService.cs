using IbnAlZumar.API.DTOs.Purchasing;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.Domain.Entities.Inventory;
using IbnAlZumar.Domain.Entities.Purchasing;
using IbnAlZumar.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace IbnAlZumar.API.Services.Purchasing
{
    public class PurchasingService : IPurchasingService
    {
        private readonly ApplicationDbContext _context;

        public PurchasingService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // Suppliers
        // ============================================================

        public async Task<List<SupplierResponseDto>> GetSuppliersAsync()
        {
            return await _context.Suppliers
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .Select(s => new SupplierResponseDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    ContactPerson = s.ContactPerson,
                    Phone = s.Phone,
                    Email = s.Email,
                    Address = s.Address,
                    TaxId = s.TaxId,
                    CurrentBalance = s.CurrentBalance,
                    TotalPurchaseOrders = s.PurchaseOrders.Count,
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<SupplierResponseDto> GetSupplierByIdAsync(int id)
        {
            var supplier = await _context.Suppliers
                .AsNoTracking()
                .Include(s => s.PurchaseOrders)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (supplier == null)
                throw new KeyNotFoundException($"لم يتم العثور على مورد بالرقم {id}");

            return MapSupplier(supplier);
        }

        public async Task<SupplierResponseDto> CreateSupplierAsync(CreateSupplierDto dto)
        {
            var supplier = new Supplier
            {
                Name = dto.Name,
                ContactPerson = dto.ContactPerson,
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                TaxId = dto.TaxId,
                CurrentBalance = 0
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            return MapSupplier(supplier);
        }

        public async Task<SupplierResponseDto> UpdateSupplierAsync(int id, UpdateSupplierDto dto)
        {
            var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.Id == id);
            if (supplier == null)
                throw new KeyNotFoundException($"لم يتم العثور على مورد بالرقم {id}");

            supplier.Name = dto.Name;
            supplier.ContactPerson = dto.ContactPerson;
            supplier.Phone = dto.Phone;
            supplier.Email = dto.Email;
            supplier.Address = dto.Address;
            supplier.TaxId = dto.TaxId;
            supplier.CurrentBalance = dto.CurrentBalance;

            await _context.SaveChangesAsync();

            return MapSupplier(supplier);
        }

        public async Task DeleteSupplierAsync(int id)
        {
            var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.Id == id);
            if (supplier == null)
                throw new KeyNotFoundException($"لم يتم العثور على مورد بالرقم {id}");

            var hasOrders = await _context.PurchaseOrders.AnyAsync(o => o.SupplierId == id);
            if (hasOrders)
                throw new InvalidOperationException("لا يمكن حذف مورد مرتبط بأوامر شراء سابقة");

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();
        }

        private static SupplierResponseDto MapSupplier(Supplier s) => new()
        {
            Id = s.Id,
            Name = s.Name,
            ContactPerson = s.ContactPerson,
            Phone = s.Phone,
            Email = s.Email,
            Address = s.Address,
            TaxId = s.TaxId,
            CurrentBalance = s.CurrentBalance,
            TotalPurchaseOrders = s.PurchaseOrders?.Count ?? 0,
            CreatedAt = s.CreatedAt
        };

        // ============================================================
        // Purchase Orders
        // ============================================================

        public async Task<List<PurchaseOrderResponseDto>> GetPurchaseOrdersAsync()
        {
            var orders = await _context.PurchaseOrders
                .AsNoTracking()
                .Include(o => o.Supplier)
                .Include(o => o.Warehouse)
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders.Select(MapPurchaseOrder).ToList();
        }

        public async Task<PurchaseOrderResponseDto> GetPurchaseOrderByIdAsync(int id)
        {
            var order = await _context.PurchaseOrders
                .AsNoTracking()
                .Include(o => o.Supplier)
                .Include(o => o.Warehouse)
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                throw new KeyNotFoundException($"لم يتم العثور على أمر شراء بالرقم {id}");

            return MapPurchaseOrder(order);
        }

        public async Task<PurchaseOrderResponseDto> CreatePurchaseOrderAsync(CreatePurchaseOrderDto dto)
        {
            var supplierExists = await _context.Suppliers.AnyAsync(s => s.Id == dto.SupplierId);
            if (!supplierExists)
                throw new KeyNotFoundException("المورد المحدد غير موجود");

            var warehouseExists = await _context.Warehouses.AnyAsync(w => w.Id == dto.WarehouseId);
            if (!warehouseExists)
                throw new KeyNotFoundException("المستودع المحدد غير موجود");

            var productIds = dto.Items.Select(i => i.ProductId).Distinct().ToList();
            var validProductCount = await _context.Products.CountAsync(p => productIds.Contains(p.Id));
            if (validProductCount != productIds.Count)
                throw new KeyNotFoundException("أحد المنتجات المحددة غير موجود");

            var order = new PurchaseOrder
            {
                PurchaseOrderNumber = dto.PurchaseOrderNumber,
                SupplierId = dto.SupplierId,
                WarehouseId = dto.WarehouseId,
                OrderDate = dto.OrderDate,
                ExpectedDeliveryDate = dto.ExpectedDeliveryDate,
                Notes = dto.Notes,
                Status = PurchaseOrderStatus.Draft,
                Items = dto.Items.Select(i => new PurchaseOrderItem
                {
                    ProductId = i.ProductId,
                    QuantityOrdered = i.QuantityOrdered,
                    QuantityReceived = 0,
                    UnitCostPrice = i.UnitCostPrice,
                    LineTotal = i.QuantityOrdered * i.UnitCostPrice
                }).ToList()
            };

            order.TotalCost = order.Items.Sum(i => i.LineTotal);

            _context.PurchaseOrders.Add(order);
            await _context.SaveChangesAsync();

            return await GetPurchaseOrderByIdAsync(order.Id);
        }

        public async Task<PurchaseOrderResponseDto> ReceivePurchaseOrderAsync(ApprovePurchaseOrderDto dto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var order = await _context.PurchaseOrders
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == dto.PurchaseOrderId);

            if (order == null)
                throw new KeyNotFoundException("أمر الشراء غير موجود");

            if (order.Status == PurchaseOrderStatus.Received)
                throw new InvalidOperationException("تم استلام أمر الشراء هذا بالفعل");

            var now = dto.ReceivedDate == default ? DateTime.UtcNow : dto.ReceivedDate;

            foreach (var item in order.Items)
            {
                var stock = await _context.ProductStocks
                    .FirstOrDefaultAsync(s => s.ProductId == item.ProductId && s.WarehouseId == order.WarehouseId);

                if (stock == null)
                {
                    stock = new ProductStock
                    {
                        ProductId = item.ProductId,
                        WarehouseId = order.WarehouseId,
                        QuantityOnHand = 0,
                        ReorderLevel = item.Product.MinStockThreshold
                    };
                    _context.ProductStocks.Add(stock);
                }

                stock.QuantityOnHand += item.QuantityOrdered;
                stock.LastRestockedAt = now;

                item.QuantityReceived = item.QuantityOrdered;

                _context.InventoryTransactions.Add(new InventoryTransaction
                {
                    ProductId = item.ProductId,
                    WarehouseId = order.WarehouseId,
                    TransactionType = InventoryTransactionType.Purchase,
                    QuantityChange = item.QuantityOrdered,
                    ReferenceType = "PurchaseOrder",
                    ReferenceId = order.Id,
                    TransactionDate = now,
                    Notes = $"استلام أمر شراء رقم {order.PurchaseOrderNumber}"
                });

                // Latest purchase cost becomes the product's current cost price.
                item.Product.CurrentCostPrice = item.UnitCostPrice;
            }

            order.Status = PurchaseOrderStatus.Received;
            order.ReceivedDate = now;

            var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.Id == order.SupplierId);
            if (supplier != null)
            {
                supplier.CurrentBalance += order.TotalCost;

                _context.SupplierLedgerEntries.Add(new SupplierLedgerEntry
                {
                    SupplierId = supplier.Id,
                    TransactionType = SupplierLedgerTransactionType.PurchaseInvoice,
                    Amount = order.TotalCost,
                    RunningBalance = supplier.CurrentBalance,
                    RelatedPurchaseOrderId = order.Id,
                    TransactionDate = now,
                    Notes = $"فاتورة شراء رقم {order.PurchaseOrderNumber}"
                });
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return await GetPurchaseOrderByIdAsync(order.Id);
        }

        private static PurchaseOrderResponseDto MapPurchaseOrder(PurchaseOrder order) => new()
        {
            Id = order.Id,
            PurchaseOrderNumber = order.PurchaseOrderNumber,
            SupplierId = order.SupplierId,
            SupplierName = order.Supplier?.Name ?? string.Empty,
            WarehouseId = order.WarehouseId,
            WarehouseName = order.Warehouse?.Name ?? string.Empty,
            Status = order.Status.ToString(),
            OrderDate = order.OrderDate,
            ExpectedDeliveryDate = order.ExpectedDeliveryDate,
            ReceivedDate = order.ReceivedDate,
            TotalCost = order.TotalCost,
            Notes = order.Notes,
            Items = order.Items.Select(i => new PurchaseOrderItemResponseDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? string.Empty,
                ProductNameAr = i.Product?.NameAr,
                SKU = i.Product?.SKU ?? string.Empty,
                QuantityOrdered = i.QuantityOrdered,
                QuantityReceived = i.QuantityReceived,
                UnitCostPrice = i.UnitCostPrice,
                LineTotal = i.LineTotal
            }).ToList()
        };

        // ============================================================
        // Supplier Accounting (Ledger & Payments)
        // ============================================================

        public async Task<SupplierPaymentResponseDto> CreateSupplierPaymentAsync(CreateSupplierPaymentDto dto)
        {
            var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.Id == dto.SupplierId);
            if (supplier == null)
                throw new KeyNotFoundException($"لم يتم العثور على مورد بالرقم {dto.SupplierId}");

            if (dto.PurchaseOrderId.HasValue)
            {
                var orderBelongsToSupplier = await _context.PurchaseOrders
                    .AnyAsync(o => o.Id == dto.PurchaseOrderId.Value && o.SupplierId == dto.SupplierId);

                if (!orderBelongsToSupplier)
                    throw new KeyNotFoundException("أمر الشراء المحدد غير موجود أو لا ينتمي لهذا المورد");
            }

            if (!Enum.TryParse<SupplierPaymentMethod>(dto.PaymentMethod, ignoreCase: true, out var paymentMethod))
                throw new ArgumentException("طريقة الدفع غير صالحة. القيم المسموحة: Cash, BankTransfer, Cheque");

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var paymentDate = dto.PaymentDate == default ? DateTime.UtcNow : dto.PaymentDate;

            var payment = new SupplierPayment
            {
                SupplierId = dto.SupplierId,
                PurchaseOrderId = dto.PurchaseOrderId,
                Amount = dto.Amount,
                PaymentMethod = paymentMethod,
                PaymentDate = paymentDate,
                Notes = dto.Notes,
                CreatedByUserId = dto.CreatedByUserId
            };

            _context.SupplierPayments.Add(payment);

            supplier.CurrentBalance -= dto.Amount;

            // Save first so the payment gets its Id (needed for the ledger's RelatedPaymentId FK).
            await _context.SaveChangesAsync();

            _context.SupplierLedgerEntries.Add(new SupplierLedgerEntry
            {
                SupplierId = supplier.Id,
                TransactionType = SupplierLedgerTransactionType.Payment,
                Amount = -dto.Amount,
                RunningBalance = supplier.CurrentBalance,
                RelatedPurchaseOrderId = dto.PurchaseOrderId,
                RelatedPaymentId = payment.Id,
                TransactionDate = paymentDate,
                Notes = dto.Notes ?? $"دفعة مورد رقم {payment.Id}"
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return await MapSupplierPaymentAsync(payment.Id);
        }

        public async Task<List<SupplierLedgerEntryDto>> GetSupplierLedgerAsync(int supplierId)
        {
            var supplierExists = await _context.Suppliers.AnyAsync(s => s.Id == supplierId);
            if (!supplierExists)
                throw new KeyNotFoundException($"لم يتم العثور على مورد بالرقم {supplierId}");

            return await _context.SupplierLedgerEntries
                .AsNoTracking()
                .Include(l => l.RelatedPurchaseOrder)
                .Where(l => l.SupplierId == supplierId)
                .OrderBy(l => l.TransactionDate).ThenBy(l => l.Id)
                .Select(l => new SupplierLedgerEntryDto
                {
                    Id = l.Id,
                    SupplierId = l.SupplierId,
                    TransactionType = l.TransactionType.ToString(),
                    Amount = l.Amount,
                    RunningBalance = l.RunningBalance,
                    RelatedPurchaseOrderId = l.RelatedPurchaseOrderId,
                    RelatedPurchaseOrderNumber = l.RelatedPurchaseOrder != null ? l.RelatedPurchaseOrder.PurchaseOrderNumber : null,
                    RelatedPaymentId = l.RelatedPaymentId,
                    TransactionDate = l.TransactionDate,
                    Notes = l.Notes,
                    CreatedAt = l.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<SupplierDetailsDto> GetSupplierDetailsAsync(int supplierId)
        {
            var supplier = await _context.Suppliers
                .AsNoTracking()
                .Include(s => s.PurchaseOrders)
                .FirstOrDefaultAsync(s => s.Id == supplierId);

            if (supplier == null)
                throw new KeyNotFoundException($"لم يتم العثور على مورد بالرقم {supplierId}");

            var ledgerEntries = await GetSupplierLedgerAsync(supplierId);

            var payments = await _context.SupplierPayments
                .AsNoTracking()
                .Include(p => p.PurchaseOrder)
                .Where(p => p.SupplierId == supplierId)
                .OrderByDescending(p => p.PaymentDate)
                .Select(p => new SupplierPaymentResponseDto
                {
                    Id = p.Id,
                    SupplierId = p.SupplierId,
                    SupplierName = supplier.Name,
                    PurchaseOrderId = p.PurchaseOrderId,
                    PurchaseOrderNumber = p.PurchaseOrder != null ? p.PurchaseOrder.PurchaseOrderNumber : null,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod.ToString(),
                    PaymentDate = p.PaymentDate,
                    Notes = p.Notes,
                    CreatedByUserId = p.CreatedByUserId,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            return new SupplierDetailsDto
            {
                Id = supplier.Id,
                Name = supplier.Name,
                ContactPerson = supplier.ContactPerson,
                Phone = supplier.Phone,
                Email = supplier.Email,
                Address = supplier.Address,
                TaxId = supplier.TaxId,
                CurrentBalance = supplier.CurrentBalance,
                TotalPurchaseOrders = supplier.PurchaseOrders?.Count ?? 0,
                CreatedAt = supplier.CreatedAt,
                LedgerEntries = ledgerEntries,
                Payments = payments
            };
        }

        private async Task<SupplierPaymentResponseDto> MapSupplierPaymentAsync(int paymentId)
        {
            var payment = await _context.SupplierPayments
                .AsNoTracking()
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseOrder)
                .FirstOrDefaultAsync(p => p.Id == paymentId);

            if (payment == null)
                throw new KeyNotFoundException("لم يتم العثور على الدفعة");

            return new SupplierPaymentResponseDto
            {
                Id = payment.Id,
                SupplierId = payment.SupplierId,
                SupplierName = payment.Supplier?.Name ?? string.Empty,
                PurchaseOrderId = payment.PurchaseOrderId,
                PurchaseOrderNumber = payment.PurchaseOrder?.PurchaseOrderNumber,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod.ToString(),
                PaymentDate = payment.PaymentDate,
                Notes = payment.Notes,
                CreatedByUserId = payment.CreatedByUserId,
                CreatedAt = payment.CreatedAt
            };
        }
    }
}