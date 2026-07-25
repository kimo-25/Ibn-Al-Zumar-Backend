using System.ComponentModel.DataAnnotations;

namespace IbnAlZumar.API.DTOs.Catalog;

    /// <summary>
    /// Fields a client is allowed to change on an existing product.
    /// Id is taken from the route, not the body.
    /// </summary>
    public class UpdateProductDto
    {
        [Required(ErrorMessage = "SKU مطلوب")]
        [MaxLength(50)]
        public string SKU { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Barcode { get; set; }

        [Required(ErrorMessage = "اسم المنتج مطلوب")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? NameAr { get; set; }

        public string? Description { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "سعر البيع يجب أن يكون أكبر من أو يساوي صفر")]
        public decimal SellingPrice { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "سعر التكلفة يجب أن يكون أكبر من أو يساوي صفر")]
        public decimal CurrentCostPrice { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "عدد القطع في الكرتونة يجب أن يكون 1 على الأقل")]
        public int QuantityPerCarton { get; set; } = 1;

        public bool IsActive { get; set; } = true;

        public bool TrackInventory { get; set; } = true;

        [Required(ErrorMessage = "التصنيف مطلوب")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "البراند مطلوب")]
        public int BrandId { get; set; }
    }
