using IbnAlZumar.API.DTOs.Catalog;
using IbnAlZumar.API.DTOs.Common;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace IbnAlZumar.API.Services.Catalog;

/// <summary>
/// Backs the POS "Horizontal Grid View" search box — one round trip returns everything the
/// cashier screen needs to render a product tile and open the variant/unit picker: the
/// product's units, active variants, configured pricing-tier breaks, and its on-hand quantity
/// in the requested warehouse (defaults to Warehouse.Id = 1, always a valid default per
/// ARCHITECTURE.md §7.10).
/// </summary>
public class PosCatalogService : IPosCatalogService
{
    private readonly ApplicationDbContext _context;

    public PosCatalogService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IbnAlZumar.API.DTOs.Common.PagedResultDto<PosProductDto>> SearchAsync(
        string? query,
        int warehouseId,
        PricingTierType tier,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize is < 1 or > 200 ? 30 : pageSize;

        var baseQuery = _context.Products.AsNoTracking().Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim().ToLower();
            baseQuery = baseQuery.Where(p =>
                p.Name.ToLower().Contains(term) ||
                (p.NameAr != null && p.NameAr.ToLower().Contains(term)) ||
                p.SKU.ToLower().Contains(term) ||
                (p.Barcode != null && p.Barcode.ToLower() == term));
        }

        var totalCount = await baseQuery.CountAsync(ct);

        // Project straight to the DTO shape (avoid loading full navigation graphs — §4.5).
        var items = await baseQuery
            .OrderBy(p => p.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PosProductDto
            {
                Id = p.Id,
                Sku = p.SKU,
                Barcode = p.Barcode,
                Name = p.Name,
                NameAr = p.NameAr,
                ImageUrl = p.ImageUrl,
                SellingPrice = p.SellingPrice,
                TrackInventory = p.TrackInventory,
                QuantityOnHand = p.Stocks
                    .Where(s => s.WarehouseId == warehouseId)
                    .Select(s => (decimal?)s.QuantityOnHand)
                    .FirstOrDefault() ?? 0m,
                Units = p.UnitConversions
                    .Select(u => new PosUnitConversionDto
                    {
                        Id = u.Id,
                        FromUnit = u.FromUnit,
                        ToUnit = u.ToUnit,
                        Factor = u.Factor,
                        IsBaseUnit = u.IsBaseUnit
                    })
                    .ToList(),
                Variants = p.Variants
                    .Where(v => v.IsActive)
                    .Select(v => new PosVariantDto
                    {
                        Id = v.Id,
                        Sku = v.SKU,
                        Price = v.Price,
                        StockQuantity = v.StockQuantity,
                        Color = v.Color,
                        Finish = v.Finish,
                        Material = v.Material,
                        Size = v.Size,
                        IsActive = v.IsActive
                    })
                    .ToList(),
                PricingTiers = p.Prices
                    .Where(pp => pp.IsActive)
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
                    .ToList()
            })
            .ToListAsync(ct);

        return new IbnAlZumar.API.DTOs.Common.PagedResultDto<PosProductDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }
}
