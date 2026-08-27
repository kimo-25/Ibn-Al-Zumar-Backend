using IbnAlZumar.API.DTOs.Sales;
using IbnAlZumar.Domain.Enums;

namespace IbnAlZumar.API.Services.Payments;

public interface IPaymobService
{
    Task<PaymobCheckoutResult> CreateCheckoutAsync(OrderResponseDto order, CreateOrderDto request, CancellationToken cancellationToken = default);
    bool VerifyCallbackHmac(PaymobCallbackTransaction transaction, string providedHmac);
    int GetIntegrationId(PaymentMethod paymentMethod);
}
