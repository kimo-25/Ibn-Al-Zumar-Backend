using IbnAlZumar.API.DTOs.Payments;
using IbnAlZumar.API.DTOs.Sales;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.API.Services.Payments;
using IbnAlZumar.Domain.Entities.Sales;
using IbnAlZumar.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Sales;

namespace IbnAlZumar.API.Controllers;

[ApiController]
[Route("api/payments")]
public sealed class PaymentsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IOrderService _orders;
    private readonly IPaymobService _paymob;

    public PaymentsController(ApplicationDbContext db, IOrderService orders, IPaymobService paymob)
    {
        _db = db;
        _orders = orders;
        _paymob = paymob;
    }

    [HttpPost("checkout")]
    [ProducesResponseType(typeof(PaymentCheckoutResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Checkout([FromBody] CreateOrderDto request, CancellationToken cancellationToken)
    {
        if (request.Items is null || request.Items.Count == 0) return BadRequest(new { message = "السلة فارغة." });
        var electronic = request.PaymentMethod is PaymentMethod.CreditCard or PaymentMethod.InstaPay or PaymentMethod.Wallet;
        var order = await _orders.CreateAsync(request);
        var entity = await _db.Orders.FirstAsync(x => x.Id == order.Id, cancellationToken);

        if (!electronic)
        {
            return Ok(new PaymentCheckoutResponseDto
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                PaymentStatus = entity.PaymentStatus.ToString(),
                TotalAmount = order.TotalAmount
            });
        }

        var checkout = await _paymob.CreateCheckoutAsync(order, request, cancellationToken);
        entity.PaymobOrderId = checkout.PaymobOrderId;
        await _db.SaveChangesAsync(cancellationToken);
        return Ok(new PaymentCheckoutResponseDto
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
            PaymentStatus = entity.PaymentStatus.ToString(),
            PaymentUrl = checkout.PaymentUrl,
            TotalAmount = order.TotalAmount
        });
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook([FromBody] PaymentWebhookRequest request, CancellationToken cancellationToken)
    {
        if (request.Obj is null || string.IsNullOrWhiteSpace(request.Hmac)) return BadRequest();
        var transaction = new PaymobCallbackTransaction
        {
            Id = request.Obj.Id,
            AmountCents = request.Obj.AmountCents,
            CreatedAt = request.Obj.CreatedAt,
            Currency = request.Obj.Currency,
            ErrorOccured = request.Obj.ErrorOccured,
            HasParentTransaction = request.Obj.HasParentTransaction,
            IntegrationId = request.Obj.IntegrationId,
            Is3dSecure = request.Obj.Is3dSecure,
            IsAuth = request.Obj.IsAuth,
            IsCapture = request.Obj.IsCapture,
            IsRefunded = request.Obj.IsRefunded,
            IsStandalonePayment = request.Obj.IsStandalonePayment,
            IsVoided = request.Obj.IsVoided,
            Owner = request.Obj.Owner,
            Pending = request.Obj.Pending,
            Success = request.Obj.Success,
            Order = new PaymobCallbackOrder { Id = request.Obj.Order.Id },
            SourceData = new PaymobSourceData { Pan = request.Obj.SourceData.Pan, SubType = request.Obj.SourceData.SubType, Type = request.Obj.SourceData.Type }
        };
        if (!_paymob.VerifyCallbackHmac(transaction, request.Hmac)) return Unauthorized(new { message = "Invalid HMAC." });

        var order = await _db.Orders.FirstOrDefaultAsync(x => x.PaymobOrderId == request.Obj.Order.Id.ToString() || x.Id == request.Obj.Order.Id, cancellationToken);
        if (order is null) return NotFound();
        order.PaymobTransactionId = request.Obj.Id.ToString();
        order.PaymentStatus = request.Obj.Success && !request.Obj.Pending ? PaymentStatus.Paid : PaymentStatus.Failed;
        var payment = await _db.Payments.FirstOrDefaultAsync(x => x.PaymobTransactionId == order.PaymobTransactionId, cancellationToken);
        if (payment is null)
        {
            _db.Payments.Add(new Payment { OrderId = order.Id, Amount = order.TotalAmount, Method = order.PaymentMethod, Status = order.PaymentStatus, PaymobTransactionId = order.PaymobTransactionId, Notes = "Paymob webhook" });
        }
        else payment.Status = order.PaymentStatus;
        await _db.SaveChangesAsync(cancellationToken);
        return Ok(new { received = true });
    }
}