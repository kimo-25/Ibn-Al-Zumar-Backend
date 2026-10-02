using IbnAlZumar.Domain.Common;
using IbnAlZumar.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace IbnAlZumar.Domain.Entities.Catalog;

/// <summary>
/// Implements the "PricingTier" design from ARCHITECTURE.md §3.3 (previously [PLANNED]).
/// A row here is one price break: for a given Product (and optionally a specific
/// ProductVariant), at a given PricingTierType, once the sold quantity (expressed in the
/// product's BASE unit — see UnitConversion) reaches MinQuantity.
///
/// Resolution rules (see PricingService.ResolveUnitPriceAsync):
///  - ProductVariantId == null applies to every variant of the product unless a variant has
///    its own override row for the same Tier/MinQuantity.
///  - Among rows that qualify for a given quantity, the highest MinQuantity wins (best
///    matching price break).
///  - If no row matches, fall back to ProductVariant.Price, then Product.SellingPrice.
///
/// Never re-resolve the price of a historical order from this table — Order.PricingTier and
/// OrderItem.UnitPrice are frozen at sale time (ARCHITECTURE.md §7.4).
/// </summary>
public class ProductPrice : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    /// <summary>Null = applies to the product / every variant. Set = overrides for one variant only.</summary>
    public int? ProductVariantId { get; set; }
    public ProductVariant? ProductVariant { get; set; }

    public PricingTierType Tier { get; set; } = PricingTierType.Retail;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    /// <summary>Quantity threshold (in the product's base unit) from which this price applies.</summary>
    public int MinQuantity { get; set; } = 1;

    public bool IsActive { get; set; } = true;
}
