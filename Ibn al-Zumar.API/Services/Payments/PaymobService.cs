using System.Globalization;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using DTOs.Sales;
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
            PaymentMethod.ApplePay => _options.WalletIntegrationId, // map ApplePay to Wallet integration id (adjust if you have a dedicated id)
            _ => throw new InvalidOperationException("ÿ—Ìﬁ… «·œ›⁄ «·≈·ﬂ —Ê‰Ì… €Ì— „œ⁄Ê„… ›Ì ≈⁄œ«œ«  Paymob.")
        };
        return int.TryParse(value, out var id) && id > 0
            ? id
            : throw new InvalidOperationException($"Integration ID €Ì— „÷»Êÿ ·ÿ—Ìﬁ… «·œ›⁄ {paymentMethod}.");
    }

    // ... rest unchanged ...
}