// File: IbnAlZumar.API/DTOs/Sales/CreateOrderDto.cs
using IbnAlZumar.Domain.Enums;

namespace IbnAlZumar.API.DTOs.Sales
{
    public class CreateOrderDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string? CustomerEmail { get; set; }
        public string? ShippingAddress { get; set; }
        public int? ShippingZoneId { get; set; }
        public string? Notes { get; set; }

        // --- حقول المنطقة الجديدة المطلوبة من العميل ---
        public bool IsCustomZoneRequested { get; set; }
        public string? CustomZoneName { get; set; }

        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
        public OrderSource OrderSource { get; set; } = OrderSource.Online;
        public decimal DiscountAmount { get; set; } = 0;
        public int? CustomerId { get; set; }

        public List<CreateOrderItemDto> Items { get; set; } = new();
    }

    public class CreateOrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class OrderResponseDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string? ShippingAddress { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string? PaymobTransactionId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}