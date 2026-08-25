namespace IbnAlZumar.API.DTOs.Sales
{
    public class PendingZoneRequestDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomZoneName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string? ShippingAddress { get; set; }
        public string? DeliveryGovernorate { get; set; }
        public DateTime RequestedAt { get; set; }
    }

    public class AcceptZoneRequestDto
    {
        public string? Name { get; set; }
        public string Governorate { get; set; } = string.Empty;
        public decimal ShippingCost { get; set; }
        public decimal ShippingFee { get; set; }
        public int EstimatedDays { get; set; } = 1;
    }

    public class RejectZoneRequestDto
    {
        public string? Reason { get; set; }
    }
}