using IbnAlZumar.API.DTOs.Catalog;
using IbnAlZumar.Domain.Enums;

namespace IbnAlZumar.API.Services.Catalog;

public interface IPricingService
{
    /// <summary>
    /// The single authoritative price-resolution method. quantityInBaseUnit must already be
    /// converted from the sold unit (piece/box/carton) to the product's base unit via
    /// UnitConversion — this method does not know about units, only quantities and tiers.
    /// Call this from OrderService.CreateAsync (server-side, tier-aware replacement for the
    /// current flat `Product.SellingPrice` re-fetch) — never trust a client-supplied UnitPrice.
    /// </summary>
    Task<decimal> ResolveUnitPriceAsync(
        int productId,
        int? productVariantId,
        PricingTierType tier,
        decimal quantityInBaseUnit,
        CancellationToken ct = default);

    Task<List<ProductPriceDto>> GetPricesAsync(int productId, int? productVariantId, CancellationToken ct = default);

    Task<ProductPriceDto> UpsertPriceAsync(UpsertProductPriceDto dto, CancellationToken ct = default);

    Task DeletePriceAsync(int priceId, CancellationToken ct = default);
}
