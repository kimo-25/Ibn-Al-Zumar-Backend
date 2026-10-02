using IbnAlZumar.API.Common.Exceptions;
using IbnAlZumar.API.DTOs.Catalog;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.Domain.Entities.Catalog;
using IbnAlZumar.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace IbnAlZumar.API.Services.Catalog;

public class PricingService : IPricingService
{
    private readonly ApplicationDbContext _context;

    public PricingService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<decimal> ResolveUnitPriceAsync(
        int productId,
        int? productVariantId,
        PricingTierType tier,
        decimal quantityInBaseUnit,
        CancellationToken ct = default)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Include(p => p.Prices)
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Id == productId, ct)
            ?? throw new NotFoundException("المنتج غير موجود.");

        ProductVariant? variant = productVariantId.HasValue
            ? product.Variants.FirstOrDefault(v => v.Id == productVariantId.Value)
            : null;

        if (productVariantId.HasValue && variant is null)
            throw new NotFoundException("متغيّر المنتج غير موجود.");

        // Candidate price-break rows for this tier that this quantity already qualifies for.
        // A variant-specific row always outranks a product-level row for the same tier/quantity;
        // among rows at the same specificity, the highest MinQuantity that still qualifies wins
        // (the best matching price break).
        var candidate = product.Prices
            .Where(pp => pp.IsActive
                         && pp.Tier == tier
                         && (pp.ProductVariantId == productVariantId || pp.ProductVariantId == null)
                         && pp.MinQuantity <= quantityInBaseUnit)
            .OrderByDescending(pp => pp.ProductVariantId.HasValue)
            .ThenByDescending(pp => pp.MinQuantity)
            .FirstOrDefault();

        if (candidate is not null)
            return candidate.Price;

        // No configured tier price for this quantity — fall back to the flat base price.
        // This path is only for resolving a FRESH line's price; a historical order's
        // OrderItem.UnitPrice is frozen and must never be recomputed from here.
        return variant?.Price ?? product.SellingPrice;
    }

    public async Task<List<ProductPriceDto>> GetPricesAsync(int productId, int? productVariantId, CancellationToken ct = default)
    {
        var query = _context.ProductPrices.AsNoTracking().Where(pp => pp.ProductId == productId);

        if (productVariantId.HasValue)
            query = query.Where(pp => pp.ProductVariantId == productVariantId || pp.ProductVariantId == null);

        return await query
            .OrderBy(pp => pp.Tier).ThenBy(pp => pp.MinQuantity)
            .Select(pp => new ProductPriceDto
            {
                Id = pp.Id,
                ProductId = pp.ProductId,
                ProductVariantId = pp.ProductVariantId,
                Tier = pp.Tier,
                Price = pp.Price,
                MinQuantity = pp.MinQuantity,
                IsActive = pp.IsActive
            })
            .ToListAsync(ct);
    }

    public async Task<ProductPriceDto> UpsertPriceAsync(UpsertProductPriceDto dto, CancellationToken ct = default)
    {
        var productExists = await _context.Products.AnyAsync(p => p.Id == dto.ProductId, ct);
        if (!productExists)
            throw new NotFoundException("المنتج غير موجود.");

        if (dto.ProductVariantId.HasValue)
        {
            var variantBelongsToProduct = await _context.ProductVariants
                .AnyAsync(v => v.Id == dto.ProductVariantId.Value && v.ProductId == dto.ProductId, ct);
            if (!variantBelongsToProduct)
                throw new BadRequestException("متغيّر المنتج المحدد لا ينتمي لهذا المنتج.");
        }

        ProductPrice entity;

        if (dto.Id.HasValue)
        {
            entity = await _context.ProductPrices.FirstOrDefaultAsync(pp => pp.Id == dto.Id.Value, ct)
                ?? throw new NotFoundException("شريحة السعر غير موجودة.");
        }
        else
        {
            entity = new ProductPrice { ProductId = dto.ProductId };
            _context.ProductPrices.Add(entity);
        }

        entity.ProductVariantId = dto.ProductVariantId;
        entity.Tier = dto.Tier;
        entity.Price = dto.Price;
        entity.MinQuantity = dto.MinQuantity;
        entity.IsActive = dto.IsActive;

        try
        {
            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("IX_ProductPrices") == true)
        {
            throw new BadRequestException("توجد بالفعل شريحة سعر بنفس الفئة والحد الأدنى للكمية لهذا المنتج/المتغيّر.");
        }

        return new ProductPriceDto
        {
            Id = entity.Id,
            ProductId = entity.ProductId,
            ProductVariantId = entity.ProductVariantId,
            Tier = entity.Tier,
            Price = entity.Price,
            MinQuantity = entity.MinQuantity,
            IsActive = entity.IsActive
        };
    }

    public async Task DeletePriceAsync(int priceId, CancellationToken ct = default)
    {
        var entity = await _context.ProductPrices.FirstOrDefaultAsync(pp => pp.Id == priceId, ct)
            ?? throw new NotFoundException("شريحة السعر غير موجودة.");

        _context.ProductPrices.Remove(entity);
        await _context.SaveChangesAsync(ct);
    }
}
