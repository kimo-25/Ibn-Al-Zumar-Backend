namespace IbnAlZumar.API.DTOs.Catalog;

    /// <summary>
    /// Bound from query string on GET /api/products.
    /// Accepts multiple query keys used by frontends: search, keyword, name, etc.
    /// </summary>
    public class ProductFilterDto
    {
        // Incoming query parameters (binds automatically)
        public string? Search { get; set; }
        public string? Keyword { get; set; }
        public string? Name { get; set; }

        // Backing field allows assignment (controller code may still set SearchTerm)
        private string? _searchTerm;

        /// <summary>
        /// Computed helper: prefers Search, then Keyword, then Name.
        /// If explicitly set (e.g. controller fallback logic) that value is used.
        /// </summary>
        public string? SearchTerm
        {
            get => _searchTerm ?? Search ?? Keyword ?? Name;
            set => _searchTerm = value;
        }

        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }

        // Optional textual brand filter (keeps compatibility with various frontends)
        public string? Brand { get; set; }

        // Additional attribute filters requested
        public string? Material { get; set; }
        public string? Finish { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

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
                > 100 => 100,
                _ => value
            };
        }

        /// <summary>One of: name, sellingprice, createdat. Defaults to createdat desc.</summary>
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = false;
    }