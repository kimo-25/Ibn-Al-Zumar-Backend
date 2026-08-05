using System.ComponentModel.DataAnnotations;

namespace IbnAlZumar.API.DTOs.Catalog;

public class CreateProductDto
{
    [Required, MaxLength(50)]
    public string SKU { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Barcode { get; set; }

    [Required, MaxLength(300)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? NameAr { get; set; }

    public string? Description { get; set; }

    [Range(0, double.MaxValue)]
    public decimal SellingPrice { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? CurrentCostPrice { get; set; }

    [Range(1, int.MaxValue)]
    public int QuantityPerCarton { get; set; } = 1;

    public bool IsActive { get; set; } = true;
    public bool TrackInventory { get; set; } = true;

    [Required]
    public int CategoryId { get; set; }

    public int? BrandId { get; set; }

    public string? ImageUrl { get; set; } // أضفنا هذا الحقل لتخزين مسار الصورة

    public List<CreateProductVariantDto> Variants { get; set; } = new();
}