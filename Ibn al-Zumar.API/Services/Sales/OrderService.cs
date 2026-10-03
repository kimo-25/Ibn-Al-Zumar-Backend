using IbnAlZumar.Api.Services.Email;
using IbnAlZumar.API.Common.Exceptions;
using IbnAlZumar.API.DTOs.Sales;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.API.Services.Catalog;
using IbnAlZumar.Domain.Entities.Inventory;
using IbnAlZumar.Domain.Entities.Sales;
using IbnAlZumar.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Services.Sales;

namespace IbnAlZumar.API.Services.Sales;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IPricingService _pricingService; // 👈 حاقن خدمة التسعير الديناميكي

    private const decimal EgyptVatRate = 0.14m;

    public OrderService(
        ApplicationDbContext context,
        IEmailService emailService,
        IPricingService pricingService)
    {
        _context = context;
        _emailService = emailService;
        _pricing_service = pricingService;
    }

    public async Task<OrderResponseDto> CreateAsync(CreateOrderDto dto)
    {
        if (dto.Items is null || dto.Items.Count == 0)
        {
            throw new BadRequestException("السلة فارغة. لا يمكن إنشاء طلب بدون عناصر.");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var productIds = dto.Items.Select(i => i.ProductId).Distinct().ToList();

            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id) && p.IsActive)
                .ToDictionaryAsync(p => p.Id);

            decimal calculatedTotal = 0;
            var orderItems = new List<OrderItem>();
            var quantitiesByProduct = new Dictionary<int, int>();

            var selectedTier = dto.PricingTier != default ? dto.PricingTier : PricingTierType.Retail;

            foreach (var item in dto.Items)
            {
                if (item.Quantity <= 0)
                {
                    throw new BadRequestException($"الكمية غير صالحة للمنتج رقم {item.ProductId}. يجب أن تكون أكبر من صفر.");
                }

                if (!products.TryGetValue(item.ProductId, out var product))
                {
                    throw new NotFoundException($"المنتج رقم {item.ProductId} غير موجود أو غير متاح حالياً.");
                }

                // If item-level PricingTier provided, prefer it; otherwise fall back to order-level selectedTier
                var effectiveTier = item.PricingTier ?? selectedTier;

                // 👈 حساب السعر المعتمد ديناميكياً حسب شريحة السعر والكمية (item-level tier respected)
                var authoritativeUnitPrice = await _pricingService.ResolveUnitPriceAsync(
                    productId: item.ProductId,
                    productVariantId: item.ProductVariantId,
                    tier: effectiveTier,
                    quantityInBaseUnit: item.Quantity);

                // Fetch authoritative cost price from the product (or variant if you prefer)
                var authoritativeUnitCost = product.CurrentCostPrice;

                var lineTotal = item.Quantity * authoritativeUnitPrice;
                calculatedTotal += lineTotal;

                orderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductVariantId = item.ProductVariantId,
                    Quantity = item.Quantity,
                    UnitPrice = authoritativeUnitPrice,
                    UnitCostPrice = authoritativeUnitCost,
                    DiscountType = DiscountType.None,
                    DiscountValue = 0,
                    DiscountAmount = 0,
                    LineTotal = lineTotal
                });

                quantitiesByProduct[item.ProductId] =
                    quantitiesByProduct.GetValueOrDefault(item.ProductId) + item.Quantity;
            }

            var normalizedDiscountType = string.Equals(dto.DiscountType, "Percentage", StringComparison.OrdinalIgnoreCase) ? DiscountType.Percentage : DiscountType.FixedAmount;
            var discountValue = Math.Max(dto.DiscountValue, 0);

            var computedDiscount = normalizedDiscountType == DiscountType.Percentage
                ? calculatedTotal * Math.Min(discountValue, 100) / 100m
                : discountValue;
            computedDiscount = Math.Min(computedDiscount, calculatedTotal);

            var discountedSubtotal = Math.Max(calculatedTotal - computedDiscount, 0);

            // NEW: tax calculation logic
            decimal taxRate; // stored as decimal fraction (e.g. 0.15 for 15%)
            if (dto.IsTaxExempt == true)
            {
                taxRate = 0m;
            }
            else if (dto.TaxRatePercent.HasValue)
            {
                taxRate = dto.TaxRatePercent.Value / 100m;
            }
            else
            {
                taxRate = EgyptVatRate;
            }

            var taxAmount = Math.Round(discountedSubtotal * taxRate, 2);
            var totalAmount = discountedSubtotal + taxAmount;

            var defaultWarehouseId = await _context.Warehouses
                .Select(w => w.Id)
                .FirstOrDefaultAsync();

            if (defaultWarehouseId == 0)
            {
                defaultWarehouseId = 1;
            }

            var orderNumber = await GenerateOrderNumberAsync();

            int? customerId = null;
            if (!string.IsNullOrEmpty(dto.CustomerEmail))
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == dto.CustomerEmail);
                if (customer != null)
                {
                    customerId = customer.Id;
                }
            }

            ShippingZone? shippingZone = null;
            if (dto.ShippingZoneId.HasValue)
            {
                shippingZone = await _context.ShippingZones
                    .FirstOrDefaultAsync(z => z.Id == dto.ShippingZoneId.Value);
            }

            var order = new Order
            {
                OrderNumber = orderNumber,
                CustomerId = customerId,
                GuestName = dto.CustomerName,
                GuestPhone = dto.CustomerPhone,
                ShippingAddress = dto.ShippingAddress,
                ShippingZoneId = dto.ShippingZoneId,
                Notes = dto.Notes,
                Source = dto.OrderSource,
                PricingTier = selectedTier, // store order-level tier (items may override)
                Status = OrderStatus.PendingConfirmation,
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus = dto.PaymentMethod == PaymentMethod.CashOnDelivery || dto.PaymentMethod == PaymentMethod.Cash
                    ? PaymentStatus.CodPending
                    : PaymentStatus.Pending,
                WarehouseId = defaultWarehouseId,
                OrderDate = DateTime.UtcNow,
                SubTotal = calculatedTotal,
                DiscountType = normalizedDiscountType,
                DiscountValue = discountValue,
                DiscountAmount = computedDiscount,
                TaxRate = taxRate, // store effective tax rate (decimal fraction)
                TaxAmount = taxAmount,
                TotalAmount = totalAmount,
                Items = orderItems,
                IsCustomZoneRequested = dto.IsCustomZoneRequested,
                CustomZoneName = dto.IsCustomZoneRequested ? dto.CustomZoneName?.Trim() : null,
                CustomZoneRequestStatus = dto.IsCustomZoneRequested
                    ? CustomZoneRequestStatus.Pending
                    : CustomZoneRequestStatus.None
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var (productId, quantity) in quantitiesByProduct)
            {
                var product = products[productId];

                if (!product.TrackInventory)
                {
                    continue;
                }

                var stock = await _context.Set<ProductStock>()
                    .FirstOrDefaultAsync(s => s.ProductId == productId && s.WarehouseId == order.WarehouseId);

                if (stock is null || stock.QuantityOnHand < quantity)
                {
                    throw new BadRequestException(
                        $"الكمية المتاحة غير كافية للمنتج \"{product.Name}\" في المخزن المحدد.");
                }

                stock.QuantityOnHand -= quantity;

                _context.Set<InventoryTransaction>().Add(new InventoryTransaction
                {
                    ProductId = productId,
                    WarehouseId = order.WarehouseId,
                    TransactionType = InventoryTransactionType.SaleDeducted,
                    QuantityChange = -quantity,
                    ReferenceType = "Order",
                    ReferenceId = order.Id,
                    TransactionDate = DateTime.UtcNow,
                    Notes = $"بيع - طلب رقم {order.OrderNumber}"
                });
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new OrderResponseDto
            {
                Id = order.Id,
                CustomerName = order.GuestName ?? string.Empty,
                CustomerPhone = order.GuestPhone ?? string.Empty,
                OrderNumber = order.OrderNumber,
                SubTotal = order.SubTotal,
                DiscountAmount = order.DiscountAmount,
                TaxRate = order.TaxRate,
                TaxAmount = order.TaxAmount,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                PaymentMethod = order.PaymentMethod.ToString(),
                PaymentStatus = order.PaymentStatus.ToString(),
                PaymobTransactionId = order.PaymobTransactionId,
                CreatedAt = order.OrderDate
            };
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync();
            var innerMessage = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Database Error: {innerMessage}");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // ... rest of class unchanged ...
}