using IbnAlZumar.API.Persistence;
using IbnAlZumar.API.Services.Catalog;
using IbnAlZumar.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IbnAlZumar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // any authenticated staff member (cashier, moderator, admin) may search the POS catalog
public class PosController : ControllerBase
{
    private readonly IPosCatalogService _posCatalogService;
    private readonly IPricingService _pricingService;
    private readonly ApplicationDbContext _db;

    public PosController(
        IPosCatalogService posCatalogService,
        IPricingService pricingService,
        ApplicationDbContext db)
    {
        _posCatalogService = posCatalogService;
        _pricingService = pricingService;
        _db = db;
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

    /// <summary>
    /// GET /api/pos/products/for-labels?q=
    /// Light-weight endpoint returning Product Id, SKU, Barcode, and Name specifically for barcode label printing.
    /// </summary>
    [HttpGet("products/for-labels")]
    public async Task<IActionResult> GetProductsForLabels(
        [FromQuery] string? q,
        CancellationToken ct = default)
    {
        var query = _db.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            query = query.Where(p =>
                p.Name.Contains(term) ||
                p.SKU.Contains(term) ||
                (p.Barcode != null && p.Barcode.Contains(term)));
        }

        var list = await query
            .OrderBy(p => p.Name)
            .Select(p => new
            {
                p.Id,
                p.SKU,
                p.Barcode,
                p.Name
            })
            .ToListAsync(ct);

        return Ok(list);
    }
}