using IbnAlZumar.API.DTOs.Payments;
using IbnAlZumar.API.DTOs.Sales;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.API.Services.Payments;
using IbnAlZumar.Domain.Entities.Sales;
using IbnAlZumar.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
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

    /// <summary>
    /// «·”⁄— Ê«·≈Ã„«·Ì Ì „ «Õ ”«»Â„« œ«∆„« „‰ «·”Ì—›— œ«Œ· OrderService.CreateAsync ó ·« Ìı⁄ „œ ⁄·Ï ﬁÌ„ «·⁄„Ì·.
    /// </summary>
    [HttpPost("checkout")]
    [Authorize] // C-01: checkout must be authenticated ó prevents anonymous price-tampered orders
    [ProducesResponseType(typeof(PaymentCheckoutResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Checkout([FromBody] CreateOrderDto request, CancellationToken cancellationToken)
    {
        if (request.Items is null || request.Items.Count == 0) return BadRequest(new { message = "«·”·… ›«—€…." });
        var electronic = request.PaymentMethod is PaymentMethod.CreditCard or PaymentMethod.InstaPay or PaymentMethod.Wallet or PaymentMethod.ApplePay;
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

    // ... rest unchanged ...
}