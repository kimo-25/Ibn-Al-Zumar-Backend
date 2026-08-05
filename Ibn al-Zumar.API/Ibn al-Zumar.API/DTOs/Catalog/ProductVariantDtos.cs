using System.ComponentModel.DataAnnotations;

namespace IbnAlZumar.API.DTOs.Catalog;

public class ProductVariantResponseDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? Color { get; set; }
    public string? Finish { get; set; }
    public string? Material { get; set; }
    public bool IsActive { get; set; }
}

public class CreateProductVariantDto
{
    [Required, MaxLength(100)]
    public string SKU { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    [MaxLength(100)]
    public string? Color { get; set; }

    [MaxLength(100)]
    public string? Finish { get; set; }

    [MaxLength(100)]
    public string? Material { get; set; }

    public bool IsActive { get; set; } = true;
}

public class UpdateProductVariantDto
{
    public int? Id { get; set; } // null => create new variant on update

    [Required, MaxLength(100)]
    public string SKU { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    [MaxLength(100)]
    public string? Color { get; set; }

    [MaxLength(100)]
    public string? Finish { get; set; }

    [MaxLength(100)]
    public string? Material { get; set; }

    public bool IsActive { get; set; } = true;
}