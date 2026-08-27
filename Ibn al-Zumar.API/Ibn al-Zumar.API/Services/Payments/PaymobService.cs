using System.Globalization;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using IbnAlZumar.API.DTOs.Sales;
using IbnAlZumar.Domain.Enums;
using Microsoft.Extensions.Options;

namespace IbnAlZumar.API.Services.Payments;

public sealed class PaymobService : IPaymobService
{
    private readonly HttpClient _httpClient;
    private readonly PaymobOptions _options;

    public PaymobService(HttpClient httpClient, IOptions<PaymobOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _httpClient.BaseAddress = new Uri(_options.BaseUrl.TrimEnd('/') + "/");
    }

    public int GetIntegrationId(PaymentMethod paymentMethod)
    {
        var value = paymentMethod switch
        {
            PaymentMethod.CreditCard => _options.CardIntegrationId,
            PaymentMethod.InstaPay => _options.InstaPayIntegrationId,
            PaymentMethod.Wallet => _options.WalletIntegrationId,
            _ => throw new InvalidOperationException("طريقة الدفع الإلكترونية غير مدعومة في إعدادات Paymob.")
        };
        return int.TryParse(value, out var id) && id > 0
            ? id
            : throw new InvalidOperationException($"Integration ID غير مضبوط لطريقة الدفع {paymentMethod}.");
    }

    public async Task<PaymobCheckoutResult> CreateCheckoutAsync(OrderResponseDto order, CreateOrderDto request, CancellationToken cancellationToken = default)
    {
        EnsureConfigured();
        var auth = await PostAsync<PaymobAuthResponse>("api/auth/tokens", new { api_key = _options.ApiKey }, cancellationToken);
        var paymobOrder = await PostAsync<PaymobOrderResponse>("api/ecommerce/orders", new
        {
            auth_token = auth.Token,
            delivery_needed = false,
            amount_cents = ToCents(order.TotalAmount),
            currency = "EGP",
            merchant_order_id = order.Id.ToString(CultureInfo.InvariantCulture),
            items = request.Items.Select(item => new
            {
                name = $"Product-{item.ProductId}",
                amount_cents = ToCents(item.UnitPrice),
                description = "Ibn al-Zumar product",
                quantity = item.Quantity
            })
        }, cancellationToken);

        var paymentKey = await PostAsync<PaymobPaymentKeyResponse>("api/acceptance/payment_keys", new
        {
            auth_token = auth.Token,
            amount_cents = ToCents(order.TotalAmount),
            expiration = 3600,
            order_id = paymobOrder.Id,
            billing_data = new
            {
                apartment = "NA",
                email = request.CustomerEmail ?? "customer@ibnalzumar.com",
                floor = "NA",
                first_name = request.CustomerName,
                street = request.ShippingAddress ?? "NA",
                building = "NA",
                phone_number = request.CustomerPhone,
                shipping_method = "NA",
                postal_code = "NA",
                city = request.CustomZoneName ?? "Cairo",
                country = "EG",
                last_name = "Customer",
                state = request.CustomZoneName ?? "Cairo"
            },
            currency = "EGP",
            integration_id = GetIntegrationId(request.PaymentMethod),
            lock_order_when_paid = false
        }, cancellationToken);

        var url = $"{_options.BaseUrl.TrimEnd('/')}/api/acceptance/iframes/{_options.IframeId}?payment_token={Uri.EscapeDataString(paymentKey.Token)}";
        return new PaymobCheckoutResult { PaymentUrl = url, PaymobOrderId = paymobOrder.Id.ToString(CultureInfo.InvariantCulture), PaymentToken = paymentKey.Token };
    }

    public bool VerifyCallbackHmac(PaymobCallbackTransaction transaction, string providedHmac)
    {
        if (string.IsNullOrWhiteSpace(_options.HmacSecret) || string.IsNullOrWhiteSpace(providedHmac)) return false;
        var values = string.Join("", new[]
        {
            transaction.AmountCents.ToString(CultureInfo.InvariantCulture),
            transaction.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture),
            transaction.Currency,
            transaction.ErrorOccured.ToString().ToLowerInvariant(),
            transaction.HasParentTransaction.ToString().ToLowerInvariant(),
            transaction.Id.ToString(CultureInfo.InvariantCulture),
            transaction.IntegrationId.ToString(CultureInfo.InvariantCulture),
            transaction.Is3dSecure.ToString().ToLowerInvariant(),
            transaction.IsAuth.ToString().ToLowerInvariant(),
            transaction.IsCapture.ToString().ToLowerInvariant(),
            transaction.IsRefunded.ToString().ToLowerInvariant(),
            transaction.IsStandalonePayment.ToString().ToLowerInvariant(),
            transaction.IsVoided.ToString().ToLowerInvariant(),
            transaction.Order.Id.ToString(CultureInfo.InvariantCulture),
            transaction.Owner.ToString(CultureInfo.InvariantCulture),
            transaction.Pending.ToString().ToLowerInvariant(),
            transaction.SourceData.Pan,
            transaction.SourceData.SubType,
            transaction.SourceData.Type,
            transaction.Success.ToString().ToLowerInvariant()
        });
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_options.HmacSecret));
        var computed = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(values))).ToLowerInvariant();
        return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(computed), Encoding.UTF8.GetBytes(providedHmac.Trim().ToLowerInvariant()));
    }

    private async Task<T> PostAsync<T>(string path, object body, CancellationToken cancellationToken)
    {
        using var response = await _httpClient.PostAsJsonAsync(path, body, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Paymob API error ({(int)response.StatusCode}): {content}");
        var result = JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return result ?? throw new InvalidOperationException("Paymob returned an empty response.");
    }

    private void EnsureConfigured()
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey) || string.IsNullOrWhiteSpace(_options.IframeId))
            throw new InvalidOperationException("إعدادات Paymob غير مكتملة على الخادم.");
    }

    private static int ToCents(decimal amount) => checked((int)Math.Round(amount * 100m, MidpointRounding.AwayFromZero));
    private sealed record PaymobAuthResponse(string Token);
    private sealed record PaymobOrderResponse(int Id);
    private sealed record PaymobPaymentKeyResponse(string Token);
}
