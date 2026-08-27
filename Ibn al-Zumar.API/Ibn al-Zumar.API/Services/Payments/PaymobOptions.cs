namespace IbnAlZumar.API.Services.Payments;

public sealed class PaymobOptions
{
    public string BaseUrl { get; set; } = "https://accept.paymob.com";
    public string ApiKey { get; set; } = string.Empty;
    public string CardIntegrationId { get; set; } = string.Empty;
    public string WalletIntegrationId { get; set; } = string.Empty;
    public string InstaPayIntegrationId { get; set; } = string.Empty;
    public string IframeId { get; set; } = string.Empty;
    public string HmacSecret { get; set; } = string.Empty;
    public string CallbackBaseUrl { get; set; } = string.Empty;
}

public sealed class PaymobCheckoutResult
{
    public string PaymentUrl { get; init; } = string.Empty;
    public string PaymobOrderId { get; init; } = string.Empty;
    public string PaymentToken { get; init; } = string.Empty;
}

public sealed class PaymobCallbackPayload
{
    public PaymobCallbackTransaction obj { get; set; } = new();
    public string? Hmac { get; set; }
}

public sealed class PaymobCallbackTransaction
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
    public PaymobCallbackOrder Order { get; set; } = new();
    public int Owner { get; set; }
    public bool Pending { get; set; }
    public PaymobSourceData SourceData { get; set; } = new();
    public bool Success { get; set; }
}

public sealed class PaymobCallbackOrder
{
    public int Id { get; set; }
}

public sealed class PaymobSourceData
{
    public string Pan { get; set; } = string.Empty;
    public string SubType { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}
