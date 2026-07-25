using IbnAlZumar.Domain.Common;
using IbnAlZumar.Domain.Entities.Catalog;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IbnAlZumar.API.Domain.Entities
{
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

        public bool IsActive { get; set; } = true;

        public bool TrackInventory { get; set; } = true;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public int BrandId { get; set; }
        public Brand Brand { get; set; } = null!;

        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

        public ICollection<ProductAttributeValue> AttributeValues { get; set; } = new List<ProductAttributeValue>();
    }
}