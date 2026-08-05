using System.Linq.Expressions;
using IbnAlZumar.API.Common.Exceptions;
using IbnAlZumar.API.DTOs.Catalog;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;

namespace IbnAlZumar.API.Services.Catalog;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Projection expression used by queries to avoid loading entire entity graph
    private static readonly Expression<Func<Product, ProductResponseDto>> ProjectToDto = static p => new ProductResponseDto
    {
        Id = p.Id,
        SKU = p.SKU,
        Barcode = p.Barcode,
        Name = p.Name,
        NameAr = p.NameAr,
        Description = p.Description,
        SellingPrice = p.SellingPrice,
        CurrentCostPrice = p.CurrentCostPrice,
        QuantityPerCarton = p.QuantityPerCarton,
        IsActive = p.IsActive,
        TrackInventory = p.TrackInventory,
        CategoryId = p.CategoryId,
        CategoryName = p.Category.Name,
        BrandId = p.BrandId,
        BrandName = p.Brand.Name,
        MainImageUrl = p.ImageUrl ?? p.Images.OrderBy(i => i.DisplayOrder).ThenByDescending(i => i.IsPrimary).Select(i => i.ImageUrl).FirstOrDefault(),
        TotalStockQuantity = p.Variants.Sum(v => v.StockQuantity),
        Variants = p.Variants
            .OrderBy(v => v.Id)
            .Select(v => new ProductVariantResponseDto
            {
                Id = v.Id,
                SKU = v.SKU,
                Price = v.Price,
                StockQuantity = v.StockQuantity,
                Color = v.Color,
                Finish = v.Finish,
                Material = v.Material,
                IsActive = v.IsActive
            }).ToList(),
        CreatedAt = p.CreatedAt
    };

    public async Task<PagedResultDto<ProductResponseDto>> GetAllAsync(ProductFilterDto filter)
    {
        var query = _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Images)
            .Include(p => p.Variants)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.Trim().ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(term) ||
                (p.SKU != null && p.SKU.ToLower().Contains(term)) ||
                (p.Barcode != null && p.Barcode.ToLower().Contains(term)));
        }

        if (filter.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == filter.CategoryId.Value);

        if (filter.BrandId.HasValue)
            query = query.Where(p => p.BrandId == filter.BrandId.Value);
        else if (!string.IsNullOrWhiteSpace(filter.Brand))
        {
            var b = filter.Brand.Trim().ToLower();
            query = query.Where(p => p.Brand.Name.ToLower().Contains(b));
        }

        if (filter.MinPrice.HasValue)
            query = query.Where(p => p.SellingPrice >= filter.MinPrice.Value);

        if (filter.MaxPrice.HasValue)
            query = query.Where(p => p.SellingPrice <= filter.MaxPrice.Value);

        if (filter.IsActive.HasValue)
            query = query.Where(p => p.IsActive == filter.IsActive.Value);

        // Sorting
        query = (filter.SortBy?.ToLower(), filter.SortDescending) switch
        {
            ("name", false) => query.OrderBy(p => p.Name),
            ("name", true) => query.OrderByDescending(p => p.Name),
            ("sellingprice", false) => query.OrderBy(p => p.SellingPrice),
            ("sellingprice", true) => query.OrderByDescending(p => p.SellingPrice),
            ("createdat", false) => query.OrderBy(p => p.CreatedAt),
            ("createdat", true) => query.OrderByDescending(p => p.CreatedAt),
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
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Images)
            .Include(p => p.Variants)
            .Where(p => p.Id == id)
            .Select(ProjectToDto)
            .FirstOrDefaultAsync();

        if (product is null)
            throw new NotFoundException($"لم يتم العثور على منتج بالرقم {id}");

        return product;
    }

    public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto)
    {
        // Validate SKU uniqueness
        await EnsureSkuIsUniqueAsync(dto.SKU);

        // Basic entity creation
        var product = new Product
        {
            SKU = dto.SKU.Trim(),
            Barcode = dto.Barcode?.Trim(),
            Name = dto.Name.Trim(),
            NameAr = dto.NameAr?.Trim(),
            Description = dto.Description,
            SellingPrice = dto.SellingPrice,
            CurrentCostPrice = dto.CurrentCostPrice ?? 0m,
            QuantityPerCarton = dto.QuantityPerCarton,
            IsActive = dto.IsActive,
            TrackInventory = dto.TrackInventory,
            CategoryId = dto.CategoryId,
            BrandId = dto.BrandId ?? 0,
            ImageUrl = dto.ImageUrl
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Variants
        if (dto.Variants is { Count: > 0 })
        {
            var variants = dto.Variants.Select(v => new ProductVariant
            {
                ProductId = product.Id,
                SKU = v.SKU.Trim(),
                Price = v.Price,
                StockQuantity = v.StockQuantity,
                Color = v.Color,
                Finish = v.Finish,
                Material = v.Material,
                IsActive = v.IsActive
            }).ToList();

            _context.ProductVariants.AddRange(variants);
            await _context.SaveChangesAsync();
        }

        return await GetByIdAsync(product.Id);
    }

    public async Task<ProductResponseDto> UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = await _context.Products
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
            throw new NotFoundException($"لم يتم العثور على منتج بالرقم {id}");

        // Ensure SKU unique excluding this product
        await EnsureSkuIsUniqueAsync(dto.SKU, excludeProductId: id);

        product.SKU = dto.SKU.Trim();
        product.Barcode = dto.Barcode?.Trim();
        product.Name = dto.Name.Trim();
        product.NameAr = dto.NameAr?.Trim();
        product.Description = dto.Description;
        product.SellingPrice = dto.SellingPrice;
        product.CurrentCostPrice = dto.CurrentCostPrice ?? product.CurrentCostPrice;
        product.QuantityPerCarton = dto.QuantityPerCarton;
        product.IsActive = dto.IsActive;
        product.TrackInventory = dto.TrackInventory;
        product.CategoryId = dto.CategoryId;
        product.BrandId = dto.BrandId ?? product.BrandId;
        if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
            product.ImageUrl = dto.ImageUrl;

        // Update variants: simple replace strategy (match by Id if provided, otherwise create)
        if (dto.Variants != null)
        {
            // Build lookup for incoming variants that have Id (Update DTO variants may include Ids)
            var incomingById = dto.Variants.Where(v => v.Id.HasValue && v.Id.Value > 0).ToDictionary(v => v.Id!.Value);
            // Update existing
            foreach (var existing in product.Variants.ToList())
            {
                if (incomingById.TryGetValue(existing.Id, out var incoming))
                {
                    existing.SKU = incoming.SKU.Trim();
                    existing.Price = incoming.Price;
                    existing.StockQuantity = incoming.StockQuantity;
                    existing.Color = incoming.Color;
                    existing.Finish = incoming.Finish;
                    existing.Material = incoming.Material;
                    existing.IsActive = incoming.IsActive;
                }
                else
                {
                    // Not included — remove (soft delete not required for variants here so hard remove)
                    _context.ProductVariants.Remove(existing);
                }
            }

            // Add new variants (those with Id == 0)
            var newVariants = dto.Variants.Where(v => v.Id == 0).Select(v => new ProductVariant
            {
                ProductId = product.Id,
                SKU = v.SKU.Trim(),
                Price = v.Price,
                StockQuantity = v.StockQuantity,
                Color = v.Color,
                Finish = v.Finish,
                Material = v.Material,
                IsActive = v.IsActive
            });

            if (newVariants.Any())
                _context.ProductVariants.AddRange(newVariants);
        }

        await _context.SaveChangesAsync();

        return await GetByIdAsync(product.Id);
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product is null)
            throw new NotFoundException($"لم يتم العثور على منتج بالرقم {id}");

        product.IsDeleted = true;
        await _context.SaveChangesAsync();
    }

    private async Task EnsureSkuIsUniqueAsync(string sku, int? excludeProductId = null)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new BadRequestException("SKU مطلوب");

        var exists = await _context.Products
            .AnyAsync(p => p.SKU == sku && (!excludeProductId.HasValue || p.Id != excludeProductId.Value));

        if (exists)
            throw new BadRequestException($"يوجد منتج آخر بنفس الـ SKU: {sku}");
    }
}