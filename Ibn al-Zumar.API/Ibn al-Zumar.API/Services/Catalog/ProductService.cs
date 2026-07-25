using IbnAlZumar.Domain.Entities.Catalog;
using IbnAlZumar.API.DTOs.Catalog;
using IbnAlZumar.API.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
namespace IbnAlZumar.API.Services.Catalog
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        private static readonly Expression<Func<Product, ProductResponseDto>> ProjectToDto = static p => new ProductResponseDto
        {
            Id = p.Id,
            SKU = p.SKU,
            Barcode = p.Barcode,
            Name = p.Name,
            NameAr = p.NameAr,
            Description = p.Description,
            SellingPrice = p.SellingPrice,
            CurrentCostPrice = p.CurrentCostPrice ?? 0m,
            QuantityPerCarton = p.QuantityPerCarton,
            IsActive = p.IsActive,
            TrackInventory = p.TrackInventory,
            CategoryId = p.CategoryId,
            CategoryName = p.Category.NameAr ?? p.Category.Name,
            BrandId = p.BrandId ?? 0,
            BrandName = p.Brand != null ? p.Brand.Name : string.Empty,
            CreatedAt = p.CreatedAt
        };

        public async Task<PagedResultDto<ProductResponseDto>> GetAllAsync(ProductFilterDto filter)
        {
            var query = _context.Products.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim().ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(term) ||
                    (p.NameAr != null && p.NameAr.ToLower().Contains(term)) ||
                    p.SKU.ToLower().Contains(term) ||
                    (p.Barcode != null && p.Barcode.ToLower().Contains(term)));
            }

            if (filter.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == filter.CategoryId.Value);

            if (filter.BrandId.HasValue)
                query = query.Where(p => p.BrandId == filter.BrandId.Value);

            if (filter.IsActive.HasValue)
                query = query.Where(p => p.IsActive == filter.IsActive.Value);

            query = filter.SortBy?.Trim().ToLower() switch
            {
                "name" => filter.SortDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                "sellingprice" => filter.SortDescending ? query.OrderByDescending(p => p.SellingPrice) : query.OrderBy(p => p.SellingPrice),
                "createdat" => filter.SortDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(ProjectToDto)
                .ToListAsync();

            return new PagedResultDto<ProductResponseDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<ProductResponseDto> GetByIdAsync(int id)
        {
            var product = await _context.Products
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(ProjectToDto)
                .FirstOrDefaultAsync();

            if (product is null)
                throw new KeyNotFoundException($"لم يتم العثور على منتج بالرقم {id}");

            return product;
        }

        public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto)
        {
            await ValidateCategoryAndBrandExistAsync(dto.CategoryId, dto.BrandId);
            await EnsureSkuIsUniqueAsync(dto.SKU);

            var product = new Product
            {
                SKU = dto.SKU.Trim(),
                Barcode = dto.Barcode?.Trim(),
                Name = dto.Name.Trim(),
                NameAr = dto.NameAr?.Trim(),
                Description = dto.Description,
                SellingPrice = dto.SellingPrice,
                CurrentCostPrice = dto.CurrentCostPrice,
                QuantityPerCarton = dto.QuantityPerCarton,
                IsActive = dto.IsActive,
                TrackInventory = dto.TrackInventory,
                CategoryId = dto.CategoryId,
                BrandId = dto.BrandId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(product.Id);
        }

        public async Task<ProductResponseDto> UpdateAsync(int id, UpdateProductDto dto)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product is null)
                throw new KeyNotFoundException($"لم يتم العثور على منتج بالرقم {id}");

            await ValidateCategoryAndBrandExistAsync(dto.CategoryId, dto.BrandId);
            await EnsureSkuIsUniqueAsync(dto.SKU, excludeProductId: id);

            product.SKU = dto.SKU.Trim();
            product.Barcode = dto.Barcode?.Trim();
            product.Name = dto.Name.Trim();
            product.NameAr = dto.NameAr?.Trim();
            product.Description = dto.Description;
            product.SellingPrice = dto.SellingPrice;
            product.CurrentCostPrice = dto.CurrentCostPrice;
            product.QuantityPerCarton = dto.QuantityPerCarton;
            product.IsActive = dto.IsActive;
            product.TrackInventory = dto.TrackInventory;
            product.CategoryId = dto.CategoryId;
            product.BrandId = dto.BrandId;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(product.Id);
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product is null)
                throw new KeyNotFoundException($"لم يتم العثور على منتج بالرقم {id}");

            product.IsActive = false;
            await _context.SaveChangesAsync();
        }

        private async Task ValidateCategoryAndBrandExistAsync(int categoryId, int brandId)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == categoryId);
            if (!categoryExists)
                throw new InvalidOperationException($"التصنيف رقم {categoryId} غير موجود");

            var brandExists = await _context.Brands.AnyAsync(b => b.Id == brandId);
            if (!brandExists)
                throw new InvalidOperationException($"البراند رقم {brandId} غير موجود");
        }

        private async Task EnsureSkuIsUniqueAsync(string sku, int? excludeProductId = null)
        {
            var exists = await _context.Products
                .AnyAsync(p => p.SKU == sku && (!excludeProductId.HasValue || p.Id != excludeProductId.Value));

            if (exists)
                throw new InvalidOperationException($"يوجد بالفعل منتج بنفس الـ SKU: {sku}");
        }
    }
}