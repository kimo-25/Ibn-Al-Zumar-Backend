using IbnAlZumar.API.DTOs.Catalog;
using IbnAlZumar.API.Services.Catalog;
using IbnAlZumar.Persistence.Seed;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IbnAlZumar.API.Controllers;

// NOTE: adjust the `using IbnAlZumar.API.Persistence.Seed;` line and the policy name below if
// DataSeeder.PermissionCodes lives in a different namespace in your copy of the repo. Reusing
// the existing "ProductsEdit" policy keeps this endpoint consistent with how ProductsController's
// Update action is already gated (ARCHITECTURE.md §7.8) instead of inventing a new permission
// code that would also need a DataSeeder + migration entry.
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = DataSeeder.PermissionCodes.ProductsEdit)]
public class ProductPricingController : ControllerBase
{
    private readonly IPricingService _pricingService;

    public ProductPricingController(IPricingService pricingService)
    {
        _pricingService = pricingService;
    }

    [HttpGet("{productId:int}")]
    public async Task<IActionResult> GetPrices(int productId, [FromQuery] int? productVariantId, CancellationToken ct)
    {
        var prices = await _pricingService.GetPricesAsync(productId, productVariantId, ct);
        return Ok(prices);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert([FromBody] UpsertProductPriceDto dto, CancellationToken ct)
    {
        var result = await _pricingService.UpsertPriceAsync(dto, ct);
        return Ok(result);
    }

    [HttpDelete("{priceId:int}")]
    public async Task<IActionResult> Delete(int priceId, CancellationToken ct)
    {
        await _pricingService.DeletePriceAsync(priceId, ct);
        return NoContent();
    }
}
