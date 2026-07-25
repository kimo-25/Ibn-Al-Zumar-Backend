namespace IbnAlZumar.API.DTOs.Catalog;

    /// <summary>
    /// Bound from query string on GET /api/products.
    /// Example: /api/products?searchTerm=مفك&categoryId=3&pageNumber=1&pageSize=20
    /// </summary>
    public class ProductFilterDto
    {
        /// <summary>Matches against Name, NameAr, SKU and Barcode.</summary>
        public string? SearchTerm { get; set; }

        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public bool? IsActive { get; set; }

        private int _pageNumber = 1;
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }

        private int _pageSize = 20;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value switch
            {
                < 1 => 20,
                > 100 => 100, // hard cap to protect the API from abusive page sizes
                _ => value
            };
        }

        /// <summary>One of: name, sellingprice, createdat. Defaults to createdat desc.</summary>
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = false;
    }