using System.Collections.Generic;
using IbnAlZumar.Domain.Enums;

namespace IbnAlZumar.API.DTOs.Sales
{
    public class UpdateOrderItemDto
    {
        public int ProductId { get; set; }
        public int? ProductVariantId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateOrderDto
    {
        public List<UpdateOrderItemDto> Items { get; set; } = new();
        public string? Notes { get; set; }
        public int? ShippingZoneId { get; set; }
        public string? ShippingAddress { get; set; }
    }

    public class UpdateOrderPaymentDto
    {
        public decimal PaidAmount { get; set; }
        public PaymentMethod Method { get; set; } = PaymentMethod.Cash;
        public string? Notes { get; set; }
    }
}