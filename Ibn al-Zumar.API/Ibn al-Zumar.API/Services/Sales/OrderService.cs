using IbnAlZumar.API.Common.Exceptions;
using IbnAlZumar.API.DTOs.Sales;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.Domain.Entities.Sales;
using IbnAlZumar.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Services.Sales;
using IbnAlZumar.Api.Services.Email;

namespace IbnAlZumar.API.Services.Sales;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public OrderService(ApplicationDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<OrderResponseDto> CreateAsync(CreateOrderDto dto)
    {
        try
        {
            decimal calculatedTotal = 0;
            var orderItems = new List<OrderItem>();

            foreach (var item in dto.Items)
            {
                var lineTotal = item.Quantity * item.UnitPrice;
                calculatedTotal += lineTotal;

                orderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    DiscountType = DiscountType.None,
                    DiscountValue = 0,
                    DiscountAmount = 0,
                    LineTotal = lineTotal
                });
            }

            var defaultWarehouseId = await _context.Warehouses
                .Select(w => w.Id)
                .FirstOrDefaultAsync();

            if (defaultWarehouseId == 0)
            {
                defaultWarehouseId = 1;
            }

            var orderCount = await _context.Orders.CountAsync();
            var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{(orderCount + 1):D4}";

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
                Source = OrderSource.Online,
                Status = OrderStatus.PendingConfirmation,
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus = dto.PaymentMethod == PaymentMethod.CashOnDelivery || dto.PaymentMethod == PaymentMethod.Cash
                    ? PaymentStatus.CodPending
                    : PaymentStatus.Pending,
                WarehouseId = defaultWarehouseId,
                OrderDate = DateTime.UtcNow,
                SubTotal = calculatedTotal,
                DiscountType = DiscountType.None,
                DiscountValue = 0,
                DiscountAmount = 0,
                TotalAmount = calculatedTotal,
                Items = orderItems,
                IsCustomZoneRequested = dto.IsCustomZoneRequested,
                CustomZoneName = dto.IsCustomZoneRequested ? dto.CustomZoneName?.Trim() : null,
                CustomZoneRequestStatus = dto.IsCustomZoneRequested
                    ? CustomZoneRequestStatus.Pending
                    : CustomZoneRequestStatus.None
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return new OrderResponseDto
            {
                Id = order.Id,
                CustomerName = order.GuestName ?? string.Empty,
                CustomerPhone = order.GuestPhone ?? string.Empty,
                OrderNumber = order.OrderNumber,
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
            var innerMessage = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Database Error: {innerMessage}");
        }
    }

    public async Task<List<CustomerOrderDto>> GetMyOrdersAsync(string userEmail)
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.ShippingZone)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .AsNoTracking()
            .Where(o => o.Customer != null && o.Customer.Email == userEmail)
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

                CustomerPhone = o.GuestPhone ?? (o.Customer != null ? o.Customer.Phone : null) ?? string.Empty,

                ShippingAddress = !string.IsNullOrWhiteSpace(o.ShippingAddress)
                    ? o.ShippingAddress
                    : (o.Customer != null ? o.Customer.Address : string.Empty),

                Notes = o.Notes,
                SubTotal = o.SubTotal,
                DiscountAmount = o.DiscountAmount,
                TotalAmount = o.TotalAmount,

                ShippingCost = o.ShippingZone != null ? o.ShippingZone.ShippingCost : 0,
                ShippingFee = o.ShippingZone != null ? o.ShippingZone.ShippingFee : 0,
                CreatedAt = o.OrderDate,

                Items = o.Items.Select(i => new OrderItemDetailDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product != null ? i.Product.Name : "منتج",
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    DiscountAmount = i.DiscountAmount,
                    LineTotal = i.LineTotal
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<CustomerOrderDto> GetOrderDetailsAsync(int id, string? userEmail, bool isAdminOrMod)
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
        {
            throw new NotFoundException("الطلب غير موجود");
        }

        bool isOwner = (order.Customer != null && order.Customer.Email == userEmail) ||
                       (order.CashierUser != null && order.CashierUser.Email == userEmail);

        if (!isAdminOrMod && !isOwner)
        {
            throw new UnauthorizedAccessException();
        }

        return new CustomerOrderDto
        {
            Id = order.Id,
            OrderNumber = !string.IsNullOrEmpty(order.OrderNumber)
                ? order.OrderNumber
                : $"ORD-{order.Id}",
            Status = order.Status.ToString(),
            CustomerName = !string.IsNullOrWhiteSpace(order.Customer?.FullName)
                ? order.Customer.FullName
                : (order.GuestName ?? "عميل غير معروف"),
            CustomerEmail = order.Customer?.Email,
            CustomerPhone = !string.IsNullOrWhiteSpace(order.GuestPhone)
                ? order.GuestPhone
                : (order.Customer?.Phone ?? string.Empty),
            ShippingAddress = !string.IsNullOrWhiteSpace(order.ShippingAddress)
                ? order.ShippingAddress
                : (order.Customer?.Address ?? "العنوان غير متوفر"),
            Notes = order.Notes,
            SubTotal = order.SubTotal,
            DiscountAmount = order.DiscountAmount,
            TotalAmount = order.TotalAmount,
            ShippingCost = order.ShippingZone?.ShippingCost ?? 0,
            ShippingFee = order.ShippingZone?.ShippingFee ?? 0,
            CreatedAt = order.OrderDate,
            Items = order.Items.Select(oi => new OrderItemDetailDto
            {
                ProductId = oi.ProductId,
                ProductName = oi.Product?.Name ?? "منتج غير متوفر",
                UnitPrice = oi.UnitPrice,
                Quantity = oi.Quantity,
                DiscountAmount = oi.DiscountAmount,
                LineTotal = oi.LineTotal
            }).ToList()
        };
    }

    public async Task<IEnumerable<object>> GetAllOrdersAsync()
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.ShippingZone)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .AsNoTracking()
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                Status = o.Status.ToString(),
                TotalAmount = o.TotalAmount,
                CreatedAt = o.OrderDate,
                OrderDate = o.OrderDate,

                CustomerName = o.Customer != null ? o.Customer.FullName : o.GuestName,
                CustomerEmail = o.Customer != null ? o.Customer.Email : string.Empty,
                CustomerPhone = !string.IsNullOrWhiteSpace(o.GuestPhone)
                    ? o.GuestPhone
                    : (o.Customer != null ? o.Customer.Phone : string.Empty),

                GuestName = o.GuestName,
                GuestPhone = o.GuestPhone,
                ShippingAddress = o.ShippingAddress,

                ShippingCost = o.ShippingZone != null ? o.ShippingZone.ShippingCost : 0,
                ShippingFee = o.ShippingZone != null ? o.ShippingZone.ShippingFee : 0,
                Source = o.Source.ToString(),

                IsCustomZoneRequested = o.IsCustomZoneRequested,
                CustomZoneName = o.CustomZoneName,
                CustomZoneRequestStatus = o.CustomZoneRequestStatus.ToString(),

                Items = o.Items.Select(i => new
                {
                    i.ProductId,
                    ProductName = i.Product != null ? i.Product.Name : "منتج",
                    i.Quantity,
                    i.UnitPrice,
                    i.LineTotal
                })
            })
            .ToListAsync();
    }

    public async Task AdvanceOrderStatusAsync(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order != null)
        {
            if (order.Status == OrderStatus.PendingConfirmation)
                order.Status = OrderStatus.Processing;
            else if (order.Status == OrderStatus.Processing)
                order.Status = OrderStatus.Shipped;
            else if (order.Status == OrderStatus.Shipped)
                order.Status = OrderStatus.Delivered;

            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateOrderStatusAsync(int id, OrderStatus status)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
        {
            throw new KeyNotFoundException($"الطلب برقم {id} غير موجود.");
        }

        order.Status = status;
        await _context.SaveChangesAsync();
    }

    public async Task RequestCancelOrderAsync(int orderId, string reason, string userEmail)
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null) throw new NotFoundException("الطلب غير موجود");

        if (order.Customer?.Email != userEmail)
            throw new UnauthorizedAccessException("غير مصرح لك بتعديل هذا الطلب");

        if (order.Status != OrderStatus.PendingConfirmation && order.Status != OrderStatus.Confirmed)
            throw new BadRequestException("لا يمكن إلغاء الطلب في هذه المرحلة، يرجى التواصل مع الدعم.");

        order.Status = OrderStatus.CancellationRequested;
        order.CancellationReason = reason;

        await _context.SaveChangesAsync();
    }

    public async Task ApproveCancelOrderAsync(int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null) throw new NotFoundException("الطلب غير موجود");

        order.Status = OrderStatus.Cancelled;
        await _context.SaveChangesAsync();

        var customerEmail = order.Customer?.Email ?? "kimo34443@gmail.com";
        if (!string.IsNullOrEmpty(customerEmail))
        {
            string subject = $"تم إلغاء طلبك رقم {order.OrderNumber}";
            string htmlContent = $@"
                <div dir='rtl' style='font-family: Arial, sans-serif; text-align: right;'>
                    <h3>مرحباً {order.Customer?.FullName ?? "عميلنا العزيز"}،</h3>
                    <p>تم الموافقة على إلغاء طلبك رقم <strong>{order.OrderNumber}</strong> بنجاح بناءً على طلبك.</p>
                    <p>نتمنى أن نراكم قريباً في متجر ابن الزمر.</p>
                </div>";
            await _emailService.SendEmailAsync(customerEmail, subject, htmlContent);
        }
    }
}