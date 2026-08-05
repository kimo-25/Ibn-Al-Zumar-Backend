using IbnAlZumar.API.DTOs.Sales;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.Domain.Entities.Sales;
using IbnAlZumar.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Services.Sales
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OrderResponseDto> CreateAsync(CreateOrderDto dto)
        {
            // 1. حساب إجمالي المنتجات والتحقق من الأسعار
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

            // 2. الحصول على المستودع الافتراضي
            var defaultWarehouseId = await _context.Warehouses
                .Select(w => w.Id)
                .FirstOrDefaultAsync();

            if (defaultWarehouseId == 0)
            {
                defaultWarehouseId = 1;
            }

            // 3. إنتاج رقم الطلب
            var orderCount = await _context.Orders.CountAsync();
            var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{(orderCount + 1):D4}";

            // 3.5 البحث عن حساب العميل لربطه بالطلب عند توفر الإيميل
            int? customerId = null;
            if (!string.IsNullOrEmpty(dto.CustomerEmail))
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == dto.CustomerEmail);
                if (customer != null)
                {
                    customerId = customer.Id;
                }
            }

            // 4. إنشاء الطلب وربطه بالعميل
            var order = new Order
            {
                OrderNumber = orderNumber,
                CustomerId = customerId,
                GuestName = dto.CustomerName,
                GuestPhone = dto.CustomerPhone,
                ShippingAddress = dto.ShippingAddress,
                Notes = dto.Notes,
                Source = OrderSource.Online,
                Status = OrderStatus.PendingConfirmation,
                PaymentMethod = PaymentMethod.Cash,
                WarehouseId = defaultWarehouseId,
                OrderDate = DateTime.UtcNow,
                SubTotal = calculatedTotal,
                DiscountType = DiscountType.None,
                DiscountValue = 0,
                DiscountAmount = 0,
                TotalAmount = calculatedTotal,
                Items = orderItems
            };

            // 5. حفظ الطلب في قاعدة البيانات
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // 6. إرجاع النتيجة
            return new OrderResponseDto
            {
                Id = order.Id,
                CustomerName = order.GuestName ?? string.Empty,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.OrderDate
            };
        }

        public async Task<List<CustomerOrderDto>> GetMyOrdersAsync(string userEmail)
        {
            return await _context.Orders
                .Where(o => o.Customer != null && o.Customer.Email == userEmail)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new CustomerOrderDto
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    Status = o.Status.ToString(),
                    TotalAmount = o.TotalAmount,
                    CreatedAt = o.OrderDate
                })
                .ToListAsync();
        }

        /// <summary>
        /// دالة جلب كل الطلبات للوحة التحكم والإدارة
        /// </summary>
        public async Task<IEnumerable<object>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Customer)
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
                    GuestName = o.GuestName,
                    GuestPhone = o.GuestPhone,
                    ShippingAddress = o.ShippingAddress,
                    Source = o.Source.ToString()
                })
                .ToListAsync();
        }

        /// <summary>
        /// ترقية حالة الطلب عبر المراحل
        /// </summary>
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
    }
}