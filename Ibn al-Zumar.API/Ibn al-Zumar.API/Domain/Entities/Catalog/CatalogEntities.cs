using System.ComponentModel.DataAnnotations;
using IbnAlZumar.Domain.Common;
using IbnAlZumar.Domain.Entities.Inventory;
using IbnAlZumar.Domain.Entities.Purchasing;
using IbnAlZumar.Domain.Entities.Sales;
using IbnAlZumar.Domain.Enums;

namespace IbnAlZumar.Domain.Entities.Catalog;

/// <summary>Self-referencing hierarchy: Category -> SubCategories.</summary>
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

/// <summary>
/// Not explicitly requested, but added because filtering tools by brand (DeWalt, Bosch, Makita...)
/// is standard for a hardware/power-tools catalog and is essentially free to add now.
/// </summary>
public class Brand : BaseEntity
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? LogoUrl { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}

/// <summary>
/// EAV "schema" table: defines a dynamic spec that products CAN have (Voltage, Torque, Impact Rate...).
/// Chosen over a single JSON column because it supports faceted filtering/search in SQL
/// ("show me all 20V tools") without JSON path queries, at the cost of one extra join.
/// </summary>
public class ProductAttributeDefinition : BaseEntity
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty; // e.g. "Voltage", "Torque", "Impact Rate"

    [MaxLength(20)]
    public string? Unit { get; set; } // e.g. "V", "Nm", "bpm", "Ah"

    public AttributeDataType DataType { get; set; } = AttributeDataType.Text;

    public ICollection<ProductAttributeValue> ProductAttributeValues { get; set; } = new List<ProductAttributeValue>();
}

/// <summary>EAV "data" table: the actual value of a dynamic attribute for one product.</summary>
public class ProductAttributeValue : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int ProductAttributeDefinitionId { get; set; }
    public ProductAttributeDefinition ProductAttributeDefinition { get; set; } = null!;

    [Required, MaxLength(200)]
    public string Value { get; set; } = string.Empty; // stored as text; cast per DataType when read
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

public class Product : BaseEntity
{
    [Required, MaxLength(50)]
    public string SKU { get; set; } = string.Empty;

    /// <summary>
    /// Nullable, indexed (non-unique) — not every product has a scannable barcode yet, but the
    /// column exists now so Phase 2 POS scan-to-sell doesn't require a schema change.
    /// </summary>
    [MaxLength(50)]
    public string? Barcode { get; set; }

    [Required, MaxLength(300)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? NameAr { get; set; }

    public string? Description { get; set; }

    public decimal SellingPrice { get; set; }

    /// <summary>
    /// Denormalized "last known" cost price for quick margin display. The authoritative,
    /// historical cost-per-purchase lives on PurchaseOrderItem.
    /// </summary>
    public decimal? CurrentCostPrice { get; set; }

    public int QuantityPerCarton { get; set; }

    public bool IsActive { get; set; } = true;

    /// <summary>False for non-stocked items (e.g. services); skips inventory deduction on sale.</summary>
    public bool TrackInventory { get; set; } = true;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public int? BrandId { get; set; }
    public Brand? Brand { get; set; }

    public ICollection<ProductAttributeValue> AttributeValues { get; set; } = new List<ProductAttributeValue>();
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<ProductStock> Stocks { get; set; } = new List<ProductStock>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();
}
