using IbnAlZumar.Domain.Enums;

namespace IbnAlZumar.API.DTOs.Payments;

public sealed class PaymentCheckoutResponseDto
{
    public int OrderId { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public string PaymentStatus { get; init; } = string.Empty;
    public string? PaymentUrl { get; init; }
    public decimal TotalAmount { get; init; }
}

public sealed class PaymentWebhookRequest
{
    public PaymobWebhookObject Obj { get; set; } = new();
    public string? Hmac { get; set; }
}

public sealed class PaymobWebhookObject
{
    public int Id { get; set; }
    public int AmountCents { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Currency { get; set; } = string.Empty;
    public bool ErrorOccured { get; set; }
    public bool HasParentTransaction { get; set; }
    public int IntegrationId { get; set; }
    public bool Is3dSecure { get; set; }
    public bool IsAuth { get; set; }
    public bool IsCapture { get; set; }
    public bool IsRefunded { get; set; }
    public bool IsStandalonePayment { get; set; }
    public bool IsVoided { get; set; }
    public PaymobWebhookOrder Order { get; set; } = new();
    public int Owner { get; set; }
    public bool Pending { get; set; }
    public PaymobWebhookSourceData SourceData { get; set; } = new();
    public bool Success { get; set; }
}

public sealed class PaymobWebhookOrder { public int Id { get; set; } }
public sealed class PaymobWebhookSourceData
{
    public string Pan { get; set; } = string.Empty;
    public string SubType { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}
