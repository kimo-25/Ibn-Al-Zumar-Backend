using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IbnAlZumar.Api.Services.Email;
using IbnAlZumar.API.Common.Exceptions;
using IbnAlZumar.API.DTOs.Common;
using IbnAlZumar.API.DTOs.Sales;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.API.Services.Catalog;
using IbnAlZumar.Domain.Entities.Inventory;
using IbnAlZumar.Domain.Entities.Sales;
using IbnAlZumar.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace IbnAlZumar.Api.Services.Sales;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IPricingService _pricingService;

    private const decimal EgyptVatRate = 0.14m;

    public OrderService(
        ApplicationDbContext context,
        IEmailService emailService,
        IPricingService pricingService)
    {
        _context = context;
        _emailService = emailService;
        _pricingService = pricingService;
    }

    public async Task<OrderResponseDto> CreateAsync(CreateOrderDto dto)
    {
        if (dto.Items is null || dto.Items.Count == 0)
        {
            throw new BadRequestException(
                "السلة فارغة. لا يمكن إنشاء طلب بدون عناصر.");
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            var productIds = dto.Items
                .Select(i => i.ProductId)
                .Distinct()
                .ToList();

            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id) && p.IsActive)
                .ToDictionaryAsync(p => p.Id);

            decimal calculatedTotal = 0;
            var orderItems = new List<OrderItem>();
            var quantitiesByProduct = new Dictionary<int, int>();

            var selectedTier =
                dto.PricingTier != default
                    ? dto.PricingTier
                    : PricingTierType.Retail;

            foreach (var item in dto.Items)
            {
                if (item.Quantity <= 0)
                {
                    throw new BadRequestException(
                        $"الكمية غير صالحة للمنتج رقم {item.ProductId}. يجب أن تكون أكبر من صفر.");
                }

                if (!products.TryGetValue(item.ProductId, out var product))
                {
                    throw new NotFoundException(
                        $"المنتج رقم {item.ProductId} غير موجود أو غير متاح حالياً.");
                }

                var authoritativeUnitPrice =
                    await _pricingService.ResolveUnitPriceAsync(
                        productId: item.ProductId,
                        productVariantId: item.ProductVariantId,
                        tier: selectedTier,
                        quantityInBaseUnit: item.Quantity);

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
                    quantitiesByProduct.GetValueOrDefault(item.ProductId)
                    + item.Quantity;
            }

            var normalizedDiscountType =
                string.Equals(
                    dto.DiscountType,
                    "Percentage",
                    StringComparison.OrdinalIgnoreCase)
                    ? DiscountType.Percentage
                    : DiscountType.FixedAmount;

            var discountValue = Math.Max(dto.DiscountValue, 0);

            var computedDiscount =
                normalizedDiscountType == DiscountType.Percentage
                    ? calculatedTotal * Math.Min(discountValue, 100) / 100m
                    : discountValue;

            computedDiscount =
                Math.Min(computedDiscount, calculatedTotal);

            var discountedSubtotal =
                Math.Max(calculatedTotal - computedDiscount, 0);

            var taxAmount =
                Math.Round(discountedSubtotal * EgyptVatRate, 2);

            var totalAmount =
                discountedSubtotal + taxAmount;

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
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(
                        c => c.Email == dto.CustomerEmail);

                if (customer != null)
                {
                    customerId = customer.Id;
                }
            }

            ShippingZone? shippingZone = null;

            if (dto.ShippingZoneId.HasValue)
            {
                shippingZone = await _context.ShippingZones
                    .FirstOrDefaultAsync(
                        z => z.Id == dto.ShippingZoneId.Value);
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
                PricingTier = selectedTier,
                Status = OrderStatus.PendingConfirmation,
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus =
                    dto.PaymentMethod == PaymentMethod.CashOnDelivery ||
                    dto.PaymentMethod == PaymentMethod.Cash
                        ? PaymentStatus.CodPending
                        : PaymentStatus.Pending,
                WarehouseId = defaultWarehouseId,
                OrderDate = DateTime.UtcNow,
                SubTotal = calculatedTotal,
                DiscountType = normalizedDiscountType,
                DiscountValue = discountValue,
                DiscountAmount = computedDiscount,
                TaxRate = EgyptVatRate,
                TaxAmount = taxAmount,
                TotalAmount = totalAmount,
                Items = orderItems,
                IsCustomZoneRequested = dto.IsCustomZoneRequested,
                CustomZoneName =
                    dto.IsCustomZoneRequested
                        ? dto.CustomZoneName?.Trim()
                        : null,
                CustomZoneRequestStatus =
                    dto.IsCustomZoneRequested
                        ? CustomZoneRequestStatus.Pending
                        : CustomZoneRequestStatus.None
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var (productId, quantity) in quantitiesByProduct)
            {
                var product = products[productId];

                if (!product.TrackInventory)
                    continue;

                var stock = await _context.Set<ProductStock>()
                    .FirstOrDefaultAsync(
                        s => s.ProductId == productId &&
                             s.WarehouseId == order.WarehouseId);

                if (stock is null ||
                    stock.QuantityOnHand < quantity)
                {
                    throw new BadRequestException(
                        $"الكمية المتاحة غير كافية للمنتج \"{product.Name}\" في المخزن المحدد.");
                }

                stock.QuantityOnHand -= quantity;

                _context.Set<InventoryTransaction>().Add(
                    new InventoryTransaction
                    {
                        ProductId = productId,
                        WarehouseId = order.WarehouseId,
                        TransactionType =
                            InventoryTransactionType.SaleDeducted,
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

            var innerMessage =
                ex.InnerException?.Message ?? ex.Message;

            throw new Exception(
                $"Database Error: {innerMessage}");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<OrderResponseDto> UpdateOrderAsync(
        int id,
        UpdateOrderDto dto)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            throw new NotFoundException("الطلب غير موجود");

        if (dto.Items == null || dto.Items.Count == 0)
            throw new BadRequestException(
                "الطلب يجب أن يحتوي على عناصر.");

        var productIds = dto.Items
            .Select(i => i.ProductId)
            .Distinct()
            .ToList();

        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        decimal calculatedTotal = 0;
        var newOrderItems = new List<OrderItem>();

        foreach (var item in dto.Items)
        {
            if (item.Quantity <= 0)
            {
                throw new BadRequestException(
                    $"الكمية غير صالحة للمنتج رقم {item.ProductId}.");
            }

            if (!products.TryGetValue(item.ProductId, out var product))
            {
                throw new NotFoundException(
                    $"المنتج رقم {item.ProductId} غير موجود.");
            }

            var unitPrice =
                await _pricingService.ResolveUnitPriceAsync(
                    item.ProductId,
                    item.ProductVariantId,
                    order.PricingTier,
                    item.Quantity);

            var lineTotal = item.Quantity * unitPrice;
            calculatedTotal += lineTotal;

            newOrderItems.Add(new OrderItem
            {
                ProductId = item.ProductId,
                ProductVariantId = item.ProductVariantId,
                Quantity = item.Quantity,
                UnitPrice = unitPrice,
                UnitCostPrice = product.CurrentCostPrice,
                DiscountType = DiscountType.None,
                DiscountValue = 0,
                DiscountAmount = 0,
                LineTotal = lineTotal
            });
        }

        order.Items.Clear();
        order.Items = newOrderItems;
        order.SubTotal = calculatedTotal;

        if (dto.Notes != null)
            order.Notes = dto.Notes;

        if (dto.ShippingAddress != null)
            order.ShippingAddress = dto.ShippingAddress;

        var computedDiscount =
            order.DiscountType == DiscountType.Percentage
                ? calculatedTotal * order.DiscountValue / 100m
                : order.DiscountValue;

        computedDiscount =
            Math.Min(Math.Max(computedDiscount, 0), calculatedTotal);

        order.DiscountAmount = computedDiscount;

        var discountedSubtotal =
            Math.Max(calculatedTotal - computedDiscount, 0);

        order.TaxAmount =
            Math.Round(discountedSubtotal * order.TaxRate, 2);

        order.TotalAmount =
            discountedSubtotal + order.TaxAmount;

        await _context.SaveChangesAsync();

        return new OrderResponseDto
        {
            Id = order.Id,
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

    public async Task ApplyPaymentToOrderAsync(
        int id,
        UpdateOrderPaymentDto dto)
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            throw new NotFoundException("الطلب غير موجود");

        if (dto.PaidAmount < 0)
            throw new BadRequestException(
                "المبلغ المدفوع لا يمكن أن يكون سالباً.");

        if (dto.PaidAmount > order.TotalAmount)
            throw new BadRequestException(
                "المبلغ المدفوع لا يمكن أن يتجاوز إجمالي الطلب.");

        decimal previousPaid =
            order.PaymentStatus == PaymentStatus.Paid
                ? order.TotalAmount
                : 0;

        decimal deltaPaid =
            dto.PaidAmount - previousPaid;

        if (dto.PaidAmount >= order.TotalAmount)
        {
            order.PaymentStatus = PaymentStatus.Paid;
        }
        else
        {
            order.PaymentStatus = PaymentStatus.Pending;
        }

        if (order.Customer != null && deltaPaid != 0)
        {
            order.Customer.CurrentBalance -= deltaPaid;
        }

        await _context.SaveChangesAsync();
    }

    private async Task<string> GenerateOrderNumberAsync()
    {
        var sequenceValue = await _context.Database
            .SqlQueryRaw<long>(
                "SELECT NEXT VALUE FOR dbo.OrderNumberSeq AS [Value]")
            .SingleAsync();

        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{sequenceValue:D6}";
    }

    public async Task<List<CustomerOrderDto>> GetMyOrdersAsync(
        string userEmail)
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.ShippingZone)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .AsNoTracking()
            .Where(o =>
                o.Customer != null &&
                o.Customer.Email == userEmail)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new CustomerOrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                Status = o.Status.ToString(),

                CustomerName = o.Customer != null
                    ? (o.Customer.FullName ?? string.Empty)
                    : (o.GuestName ?? string.Empty),

                CustomerEmail = o.Customer != null
                    ? (o.Customer.Email ?? string.Empty)
                    : string.Empty,

                CustomerPhone =
                    o.GuestPhone ??
                    (o.Customer != null
                        ? o.Customer.Phone
                        : null) ??
                    string.Empty,

                ShippingAddress =
                    !string.IsNullOrWhiteSpace(o.ShippingAddress)
                        ? o.ShippingAddress
                        : (o.Customer != null
                            ? o.Customer.Address
                            : string.Empty),

                Notes = o.Notes,
                SubTotal = o.SubTotal,
                DiscountAmount = o.DiscountAmount,
                TotalAmount = o.TotalAmount,

                ShippingCost =
                    o.ShippingZone != null
                        ? o.ShippingZone.ShippingCost
                        : 0,

                ShippingFee =
                    o.ShippingZone != null
                        ? o.ShippingZone.ShippingFee
                        : 0,

                CreatedAt = o.OrderDate,

                Items = o.Items
                    .Select(i => new OrderItemDetailDto
                    {
                        ProductId = i.ProductId,
                        ProductName =
                            i.Product != null
                                ? i.Product.Name
                                : "منتج",
                        UnitPrice = i.UnitPrice,
                        UnitCostPrice = i.UnitCostPrice,
                        Quantity = i.Quantity,
                        DiscountAmount = i.DiscountAmount,
                        LineTotal = i.LineTotal
                    })
                    .ToList()
            })
            .ToListAsync();
    }

    public async Task<CustomerOrderDto> GetOrderDetailsAsync(
        int id,
        string? userEmail,
        bool isAdminOrMod)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.Customer)
            .Include(o => o.CashierUser)
            .Include(o => o.ShippingZone)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            throw new NotFoundException("الطلب غير موجود");

        bool isOwner =
            (order.Customer != null &&
             order.Customer.Email == userEmail) ||
            (order.CashierUser != null &&
             order.CashierUser.Email == userEmail);

        if (!isAdminOrMod && !isOwner)
            throw new UnauthorizedAccessException();

        return new CustomerOrderDto
        {
            Id = order.Id,
            OrderNumber =
                !string.IsNullOrEmpty(order.OrderNumber)
                    ? order.OrderNumber
                    : $"ORD-{order.Id}",
            Status = order.Status.ToString(),

            CustomerName =
                !string.IsNullOrWhiteSpace(order.Customer?.FullName)
                    ? order.Customer.FullName
                    : (order.GuestName ?? "عميل غير معروف"),

            CustomerEmail = order.Customer?.Email,

            CustomerPhone =
                !string.IsNullOrWhiteSpace(order.GuestPhone)
                    ? order.GuestPhone
                    : (order.Customer?.Phone ?? string.Empty),

            ShippingAddress =
                !string.IsNullOrWhiteSpace(order.ShippingAddress)
                    ? order.ShippingAddress
                    : (order.Customer?.Address ??
                       "العنوان غير متوفر"),

            Notes = order.Notes,
            SubTotal = order.SubTotal,
            DiscountAmount = order.DiscountAmount,
            TotalAmount = order.TotalAmount,

            ShippingCost =
                order.ShippingZone?.ShippingCost ?? 0,

            ShippingFee =
                order.ShippingZone?.ShippingFee ?? 0,

            CreatedAt = order.OrderDate,

            Items = order.Items
                .Select(oi => new OrderItemDetailDto
                {
                    ProductId = oi.ProductId,
                    ProductName =
                        oi.Product?.Name ?? "منتج غير متوفر",
                    UnitPrice = oi.UnitPrice,
                    UnitCostPrice = oi.UnitCostPrice,
                    Quantity = oi.Quantity,
                    DiscountAmount = oi.DiscountAmount,
                    LineTotal = oi.LineTotal
                })
                .ToList()
        };
    }

    public async Task<PagedResultDto<OrderListDto>> GetAllOrdersAsync(
        OrderFilterDto filter,
        CancellationToken ct = default)
    {
        filter.PageNumber =
            filter.PageNumber < 1 ? 1 : filter.PageNumber;

        filter.PageSize =
            filter.PageSize < 1
                ? 30
                : Math.Min(filter.PageSize, 200);

        var query = _context.Orders
            .AsNoTracking()
            .AsQueryable();

        if (filter.Status.HasValue)
        {
            query = query.Where(
                o => o.Status == filter.Status.Value);
        }

        if (filter.FromDate.HasValue)
        {
            var from = filter.FromDate.Value.Date;

            query = query.Where(
                o => o.OrderDate >= from);
        }

        if (filter.ToDate.HasValue)
        {
            var toExclusive =
                filter.ToDate.Value.Date.AddDays(1);

            query = query.Where(
                o => o.OrderDate < toExclusive);
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.Trim().ToLower();

            query = query.Where(o =>
                (o.OrderNumber != null &&
                 o.OrderNumber.ToLower().Contains(term)) ||

                (o.GuestName != null &&
                 o.GuestName.ToLower().Contains(term)) ||

                (o.GuestPhone != null &&
                 o.GuestPhone.ToLower().Contains(term)) ||

                (o.Customer != null &&
                 o.Customer.FullName != null &&
                 o.Customer.FullName.ToLower().Contains(term)) ||

                (o.Customer != null &&
                 o.Customer.Email != null &&
                 o.Customer.Email.ToLower().Contains(term)));
        }

        bool desc = filter.SortDescending;

        query = filter.SortBy?.ToLowerInvariant() switch
        {
            "total" or "totalamount" =>
                desc
                    ? query.OrderByDescending(o => o.TotalAmount)
                    : query.OrderBy(o => o.TotalAmount),

            "createdat" or "orderdate" =>
                desc
                    ? query.OrderByDescending(o => o.OrderDate)
                    : query.OrderBy(o => o.OrderDate),

            "ordernumber" =>
                desc
                    ? query.OrderByDescending(o => o.OrderNumber)
                    : query.OrderBy(o => o.OrderNumber),

            _ => query.OrderByDescending(o => o.OrderDate)
        };

        var totalCount =
            await query.CountAsync(ct);

        var items = await query
            .Skip(
                (filter.PageNumber - 1) *
                filter.PageSize)
            .Take(filter.PageSize)
            .Select(o => new OrderListDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                Status = o.Status.ToString(),
                TotalAmount = o.TotalAmount,
                CreatedAt = o.OrderDate,

                CustomerName =
                    o.Customer != null
                        ? (o.Customer.FullName ?? string.Empty)
                        : (o.GuestName ?? string.Empty),

                CustomerEmail =
                    o.Customer != null
                        ? (o.Customer.Email ?? string.Empty)
                        : string.Empty,

                CustomerPhone =
                    !string.IsNullOrWhiteSpace(o.GuestPhone)
                        ? o.GuestPhone
                        : (o.Customer != null
                            ? (o.Customer.Phone ?? string.Empty)
                            : string.Empty),

                ItemsCount = o.Items.Count
            })
            .ToListAsync(ct);

        return new PagedResultDto<OrderListDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalPages =
                (int)Math.Ceiling(
                    totalCount /
                    (double)filter.PageSize)
        };
    }

    public async Task AdvanceOrderStatusAsync(int id)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
            throw new NotFoundException("الطلب غير موجود.");

        if (order.Status == OrderStatus.PendingConfirmation)
            order.Status = OrderStatus.Processing;
        else if (order.Status == OrderStatus.Processing)
            order.Status = OrderStatus.Shipped;
        else if (order.Status == OrderStatus.Shipped)
            order.Status = OrderStatus.Delivered;
        else
            throw new BadRequestException(
                "لا يمكن ترقية حالة الطلب الحالية.");

        await _context.SaveChangesAsync();
    }

    public async Task UpdateOrderStatusAsync(
        int id,
        OrderStatus status)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
        {
            throw new NotFoundException(
                $"الطلب برقم {id} غير موجود.");
        }

        order.Status = status;
        await _context.SaveChangesAsync();
    }

    public async Task RequestCancelOrderAsync(
        int orderId,
        string reason,
        string userEmail)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new BadRequestException(
                "سبب الإلغاء مطلوب.");
        }

        var order = await _context.Orders
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
            throw new NotFoundException("الطلب غير موجود");

        if (order.Customer?.Email != userEmail)
        {
            throw new UnauthorizedAccessException(
                "غير مصرح لك بتعديل هذا الطلب");
        }

        if (order.Status != OrderStatus.PendingConfirmation &&
            order.Status != OrderStatus.Confirmed)
        {
            throw new BadRequestException(
                "لا يمكن إلغاء الطلب في هذه المرحلة، يرجى التواصل مع الدعم.");
        }

        order.Status = OrderStatus.CancellationRequested;
        order.CancellationReason = reason.Trim();

        await _context.SaveChangesAsync();
    }

    public async Task ApproveCancelOrderAsync(int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
            throw new NotFoundException("الطلب غير موجود");

        if (order.Status != OrderStatus.CancellationRequested)
        {
            throw new BadRequestException(
                "لا يوجد طلب إلغاء معلق لهذا الطلب.");
        }

        order.Status = OrderStatus.Cancelled;

        await _context.SaveChangesAsync();

        var customerEmail = order.Customer?.Email;

        if (!string.IsNullOrEmpty(customerEmail))
        {
            string subject =
                $"تم إلغاء طلبك رقم {order.OrderNumber}";

            string htmlContent = $@"
<div dir='rtl' style='font-family: Arial, sans-serif; text-align: right;'>
    <h3>مرحباً {order.Customer?.FullName ?? "عميلنا العزيز"}،</h3>
    <p>
        تم الموافقة على إلغاء طلبك رقم
        <strong>{order.OrderNumber}</strong>
        بنجاح بناءً على طلبك.
    </p>
    <p>نتمنى أن نراكم قريباً في متجر ابن الزمر.</p>
</div>";

            await _emailService.SendEmailAsync(
                customerEmail,
                subject,
                htmlContent);
        }
    }
}