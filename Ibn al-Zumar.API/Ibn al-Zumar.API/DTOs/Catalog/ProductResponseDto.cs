namespace IbnAlZumar.API.DTOs.Catalog;
    /// <summary>
    /// Flattened, display-ready shape of a product — includes the parent
    /// Category & Brand names so the client never has to make extra round trips.
    /// </summary>
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string? Barcode { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? NameAr { get; set; }
        public string? Description { get; set; }

        public decimal SellingPrice { get; set; }
        public decimal CurrentCostPrice { get; set; }

        public int QuantityPerCarton { get; set; }
        public bool IsActive { get; set; }
        public bool TrackInventory { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;

        /// <summary>Convenience field for list/grid views — main product image, if any.</summary>
        public string? MainImageUrl { get; set; }

        /// <summary>Sum of quantities across all warehouse/branch stock records.</summary>
        public int TotalStockQuantity { get; set; }

        public DateTime CreatedAt { get; set; }
    }
