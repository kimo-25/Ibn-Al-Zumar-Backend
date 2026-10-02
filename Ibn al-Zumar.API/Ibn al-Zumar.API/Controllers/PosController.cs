using IbnAlZumar.API.Services.Catalog;
using IbnAlZumar.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IbnAlZumar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // any authenticated staff member (cashier, moderator, admin) may search the POS catalog
public class PosController : ControllerBase
{
    private readonly IPosCatalogService _posCatalogService;
    private readonly IPricingService _pricingService;

    public PosController(IPosCatalogService posCatalogService, IPricingService pricingService)
    {
        _posCatalogService = posCatalogService;
        _pricingService = pricingService;
    }

    /// <summary>
    /// GET /api/pos/products?query=&amp;warehouseId=1&amp;tier=1&amp;pageNumber=1&amp;pageSize=30
    /// Backs the POS Horizontal Grid View search box (name/nameAr/SKU/exact-barcode match).
    /// </summary>
    [HttpGet("products")]
    public async Task<IActionResult> SearchProducts(
        [FromQuery] string? query,
        [FromQuery] int warehouseId = 1,
        [FromQuery] PricingTierType tier = PricingTierType.Retail,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 30,
        CancellationToken ct = default)
    {
        var result = await _posCatalogService.SearchAsync(query, warehouseId, tier, pageNumber, pageSize, ct);
        return Ok(result);
    }

    /// <summary>
    /// GET /api/pos/products/{productId}/unit-price?productVariantId=&amp;tier=1&amp;quantity=1
    /// Live price preview for the cart line BEFORE checkout (client convenience only — the order
    /// endpoint re-resolves this itself server-side and never trusts a client-supplied price).
    /// </summary>
    [HttpGet("products/{productId:int}/unit-price")]
    public async Task<IActionResult> GetUnitPrice(
        int productId,
        [FromQuery] int? productVariantId,
        [FromQuery] PricingTierType tier = PricingTierType.Retail,
        [FromQuery] decimal quantity = 1,
        CancellationToken ct = default)
    {
        var unitPrice = await _pricingService.ResolveUnitPriceAsync(productId, productVariantId, tier, quantity, ct);
        return Ok(new { productId, productVariantId, tier, quantity, unitPrice });
    }
}
