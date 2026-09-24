using IbnAlZumar.Domain.Common;
using IbnAlZumar.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IbnAlZumar.Domain.Entities.Catalog;

public class Category : BaseEntity
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? NameAr { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required, MaxLength(160)]
    public string Slug { get; set; } = string.Empty;

    public int? ParentCategoryId { get; set; }
    public Category? ParentCategory { get; set; }

    public ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

public class Brand : BaseEntity
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? LogoUrl { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}

public class ProductAttributeDefinition : BaseEntity
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Unit { get; set; }

    public AttributeDataType DataType { get; set; } = AttributeDataType.Text;

    public ICollection<ProductAttributeValue> ProductAttributeValues { get; set; } = new List<ProductAttributeValue>();
    public ICollection<ProductVariantAttributeValue> ProductVariantAttributeValues { get; set; } = new List<ProductVariantAttributeValue>();
}

public class ProductAttributeValue : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int ProductAttributeDefinitionId { get; set; }
    public ProductAttributeDefinition ProductAttributeDefinition { get; set; } = null!;

    [Required, MaxLength(200)]
    public string Value { get; set; } = string.Empty;
}

public class ProductImage : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    [Required, MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    public bool IsPrimary { get; set; }
    public int DisplayOrder { get; set; }
}

public class ProductVariant : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    [Required, MaxLength(100)]
    public string SKU { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    [MaxLength(100)]
    public string? Color { get; set; }

    [MaxLength(100)]
    public string? Finish { get; set; }

    [MaxLength(100)]
    public string? Material { get; set; }

    /// <summary>Sheet 1: e.g. "S/M/L", "42", "1m x 2m" — free text since sizing conventions vary per product line.</summary>
    [MaxLength(100)]
    public string? Size { get; set; }

    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Sheet 1: structured, arbitrary attribute links for this variant (beyond the fixed
    /// Color/Finish/Material/Size columns above), reusing the same ProductAttributeDefinition
    /// catalog as Product-level attributes so filters/facets stay consistent.
    /// </summary>
    public ICollection<ProductVariantAttributeValue> AttributeValues { get; set; } = new List<ProductVariantAttributeValue>();
}

/// <summary>
/// Sheet 1: many-to-many-ish link giving a ProductVariant arbitrary typed attributes
/// (e.g. "Voltage" -> "220V") drawn from the shared ProductAttributeDefinition catalog,
/// on top of the fixed Color/Finish/Material/Size columns on ProductVariant itself.
/// </summary>
public class ProductVariantAttributeValue : BaseEntity
{
    public int ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; } = null!;

    public int ProductAttributeDefinitionId { get; set; }
    public ProductAttributeDefinition ProductAttributeDefinition { get; set; } = null!;

    [Required, MaxLength(200)]
    public string Value { get; set; } = string.Empty;
}

/// <summary>
/// Sheet 1: replaces the flat Product.QuantityPerCarton for full piece/box/carton (and beyond)
/// selling. All stock (ProductStock.QuantityOnHand, ProductBatch quantities) is stored in the
/// product's base unit; conversion to/from retail (piece) or wholesale (carton/box) units happens
/// only at the POS/DTO boundary, using the row where IsBaseUnit == true as the anchor.
/// Product.QuantityPerCarton is kept for backward compatibility with existing screens/reports and
/// should be kept in sync with the "Carton -> (base unit)" conversion row when both are present.
/// </summary>
public class UnitConversion : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    /// <summary>e.g. "Piece", "Box", "Carton".</summary>
    [Required, MaxLength(50)]
    public string FromUnit { get; set; } = string.Empty;

    /// <summary>The product's base unit, e.g. "Piece".</summary>
    [Required, MaxLength(50)]
    public string ToUnit { get; set; } = string.Empty;

    /// <summary>How many ToUnit (base units) make up 1 FromUnit.</summary>
    [Column(TypeName = "decimal(18,4)")]
    public decimal Factor { get; set; } = 1m;

    /// <summary>True for the row where FromUnit == ToUnit == the product's base unit (Factor = 1).</summary>
    public bool IsBaseUnit { get; set; } = false;
}