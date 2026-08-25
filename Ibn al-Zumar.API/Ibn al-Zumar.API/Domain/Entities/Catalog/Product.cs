using IbnAlZumar.Domain.Common;
using IbnAlZumar.Domain.Entities.Inventory;
using IbnAlZumar.Domain.Entities.Purchasing;
using IbnAlZumar.Domain.Entities.Sales;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IbnAlZumar.Domain.Entities.Catalog;

public class Product : BaseEntity
{
    [Required, MaxLength(50)]
    public string SKU { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Barcode { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? NameAr { get; set; }

    public string? Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal SellingPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal CurrentCostPrice { get; set; }

    public int QuantityPerCarton { get; set; } = 1;

    // NEW: الحد الأدنى للمخزون قبل تفعيل تنبيه "نقص المخزون".
    // لو الكمية الحالية <= هذه القيمة يظهر المنتج في GET /api/inventory/low-stock
    public int MinStockThreshold { get; set; } = 5;

    public bool IsActive { get; set; } = true;

    public bool TrackInventory { get; set; } = true;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public int BrandId { get; set; }
    public Brand Brand { get; set; } = null!;

    // New scalar image URL property — used as primary stored URL when present
    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<ProductAttributeValue> AttributeValues { get; set; } = new List<ProductAttributeValue>();
    public ICollection<ProductStock> Stocks { get; set; } = new List<ProductStock>();
    public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
}