using IbnAlZumar.API.Services.Catalog;
using IbnAlZumar.Persistence.Seed;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IbnAlZumar.API.Controllers;

// NOTE: same namespace caveat as ProductPricingController — adjust the `using` if
// DataSeeder.PermissionCodes lives elsewhere in your copy of the repo.
[ApiController]
[Route("api")]
[Authorize(Policy = DataSeeder.PermissionCodes.ProductsEdit)]
public class ProductTranslationController : ControllerBase
{
    private readonly IProductTranslationService _translationService;

    public ProductTranslationController(IProductTranslationService translationService)
    {
        _translationService = translationService;
    }

    /// <summary>
    /// POST /api/products/{productId}/translate?overwrite=false
    /// overwrite=false (default): fills only whichever of Name/NameAr is currently empty.
    /// overwrite=true: explicit admin consent to replace BOTH sides with a fresh translation.
    /// </summary>
    [HttpPost("products/{productId:int}/translate")]
    public async Task<IActionResult> TranslateProduct(int productId, [FromQuery] bool overwrite = false, CancellationToken ct = default)
    {
        var result = await _translationService.TranslateProductAsync(productId, overwrite, ct);
        return Ok(result);
    }

    /// <summary>POST /api/categories/{categoryId}/translate?overwrite=false — same semantics as above.</summary>
    [HttpPost("categories/{categoryId:int}/translate")]
    public async Task<IActionResult> TranslateCategory(int categoryId, [FromQuery] bool overwrite = false, CancellationToken ct = default)
    {
        var result = await _translationService.TranslateCategoryAsync(categoryId, overwrite, ct);
        return Ok(result);
    }
}
