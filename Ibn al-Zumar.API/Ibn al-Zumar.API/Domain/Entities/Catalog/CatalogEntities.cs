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

    public bool IsActive { get; set; } = true;
}