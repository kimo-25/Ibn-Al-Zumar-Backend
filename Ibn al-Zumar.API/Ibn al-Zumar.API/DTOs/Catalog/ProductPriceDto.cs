using System.ComponentModel.DataAnnotations;
using IbnAlZumar.Domain.Enums;

namespace IbnAlZumar.API.DTOs.Catalog;

public class ProductPriceDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int? ProductVariantId { get; set; }
    public PricingTierType Tier { get; set; }
    public decimal Price { get; set; }
    public int MinQuantity { get; set; }
    public bool IsActive { get; set; }
}

public class UpsertProductPriceDto
{
    /// <summary>Omit to create a new price-break row; set to update an existing one.</summary>
    public int? Id { get; set; }

    [Required]
    public int ProductId { get; set; }

    public int? ProductVariantId { get; set; }

    [Required]
    public PricingTierType Tier { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "السعر يجب أن يكون أكبر من أو يساوي صفر")]
    public decimal Price { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "الحد الأدنى للكمية يجب أن يكون 1 على الأقل")]
    public int MinQuantity { get; set; } = 1;

    public bool IsActive { get; set; } = true;
}
