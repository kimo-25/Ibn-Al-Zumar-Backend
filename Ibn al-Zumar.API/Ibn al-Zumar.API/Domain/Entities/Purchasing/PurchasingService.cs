using IbnAlZumar.API.DTOs.Purchasing;
using IbnAlZumar.API.Persistence;
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

            return new SupplierResponseDto
            {
                Id = supplier.Id,
                Name = supplier.Name,
                ContactPerson = supplier.ContactPerson,
                Phone = supplier.Phone,
                Email = supplier.Email,
                Address = supplier.Address,
                TaxId = supplier.TaxId,
                CurrentBalance = supplier.CurrentBalance
            };
        }

        public async Task<PurchaseOrderResponseDto> CreatePurchaseOrderAsync(CreatePurchaseOrderDto dto)
        {
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
                    UnitCostPrice = i.UnitCostPrice,
                    LineTotal = i.QuantityOrdered * i.UnitCostPrice
                }).ToList()
            };

            order.TotalCost = order.Items.Sum(i => i.LineTotal);

            _context.PurchaseOrders.Add(order);
            await _context.SaveChangesAsync();

            return new PurchaseOrderResponseDto
            {
                Id = order.Id,
                PurchaseOrderNumber = order.PurchaseOrderNumber,
                SupplierId = order.SupplierId,
                WarehouseId = order.WarehouseId,
                Status = order.Status.ToString(),
                OrderDate = order.OrderDate,
                ExpectedDeliveryDate = order.ExpectedDeliveryDate,
                Notes = order.Notes,
                TotalCost = order.TotalCost,
                Items = order.Items.Select(i => new PurchaseOrderItemResponseDto
                {
                    ProductId = i.ProductId,
                    QuantityOrdered = i.QuantityOrdered,
                    QuantityReceived = i.QuantityReceived,
                    UnitCostPrice = i.UnitCostPrice,
                    LineTotal = i.LineTotal
                }).ToList()
            };
        }

        public async Task<PurchaseOrderResponseDto> ApprovePurchaseOrderAsync(ApprovePurchaseOrderDto dto)
        {
            var order = await _context.PurchaseOrders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == dto.PurchaseOrderId);

            if (order == null)
                throw new Exception("Purchase order not found");

            order.Status = PurchaseOrderStatus.Received;
            order.ReceivedDate = dto.ReceivedDate;

            // هنا تقدر تزود منطق زيادة المخزون (ProductStock) وتحديث الأسعار

            await _context.SaveChangesAsync();

            return new PurchaseOrderResponseDto
            {
                Id = order.Id,
                PurchaseOrderNumber = order.PurchaseOrderNumber,
                SupplierId = order.SupplierId,
                WarehouseId = order.WarehouseId,
                Status = order.Status.ToString(),
                OrderDate = order.OrderDate,
                ReceivedDate = order.ReceivedDate,
                Notes = order.Notes,
                TotalCost = order.TotalCost,
                Items = order.Items.Select(i => new PurchaseOrderItemResponseDto
                {
                    ProductId = i.ProductId,
                    QuantityOrdered = i.QuantityOrdered,
                    QuantityReceived = i.QuantityReceived,
                    UnitCostPrice = i.UnitCostPrice,
                    LineTotal = i.LineTotal
                }).ToList()
            };
        }
    }
}
