using System.Linq.Expressions;
using ClosedXML.Excel;
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
        await EnsureSkuIsUniqueAsync(dto.SKU);

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

        if (dto.Variants != null)
        {
            var incomingById = dto.Variants.Where(v => v.Id.HasValue && v.Id.Value > 0).ToDictionary(v => v.Id!.Value);

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
                    _context.ProductVariants.Remove(existing);
                }
            }

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

    // ==========================================
    // Bulk Import Implementation (Excel)
    // ==========================================
    public async Task<BulkImportResultDto> BulkImportAsync(Stream fileStream)
    {
        var result = new BulkImportResultDto();

        using var workbook = new XLWorkbook(fileStream);
        var worksheet = workbook.Worksheets.FirstOrDefault();
        if (worksheet is null)
            throw new BadRequestException("الملف لا يحتوي على أي ورقة عمل (Sheet) صالحة.");

        var columnMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var cell in worksheet.Row(1).CellsUsed())
        {
            var header = cell.GetString().Trim();
            if (!string.IsNullOrWhiteSpace(header) && !columnMap.ContainsKey(header))
                columnMap[header] = cell.Address.ColumnNumber;
        }

        string[] requiredColumns = { "SKU", "Name", "SellingPrice", "CategoryId" };
        var missingColumns = requiredColumns.Where(c => !columnMap.ContainsKey(c)).ToList();
        if (missingColumns.Count > 0)
            throw new BadRequestException(
                $"الأعمدة الإلزامية التالية مفقودة من الملف: {string.Join(", ", missingColumns)}. " +
                "من فضلك استخدم قالب الاكسل الرسمي.");

        int Col(string name) => columnMap.TryGetValue(name, out var idx) ? idx : -1;

        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        var existingSkuSet = new HashSet<string>(
            await _context.Products.Select(p => p.SKU).ToListAsync(),
            StringComparer.OrdinalIgnoreCase);

        var validCategoryIds = (await _context.Categories.Select(c => c.Id).ToListAsync()).ToHashSet();
        var validBrandIds = (await _context.Brands.Select(b => b.Id).ToListAsync()).ToHashSet();

        var seenSkusInFile = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var productsToInsert = new List<Product>();
        int processedRows = 0;

        for (int rowNum = 2; rowNum <= lastRow; rowNum++)
        {
            var row = worksheet.Row(rowNum);
            if (row.IsEmpty()) continue;

            processedRows++;
            var rowErrors = new List<string>();

            var sku = row.Cell(Col("SKU")).GetString().Trim();
            var name = row.Cell(Col("Name")).GetString().Trim();

            if (string.IsNullOrWhiteSpace(sku))
                rowErrors.Add("حقل SKU مطلوب.");
            else if (sku.Length > 50)
                rowErrors.Add("حقل SKU لا يجب أن يتجاوز 50 حرفاً.");

            if (string.IsNullOrWhiteSpace(name))
                rowErrors.Add("حقل Name مطلوب.");
            else if (name.Length > 300)
                rowErrors.Add("حقل Name لا يجب أن يتجاوز 300 حرف.");

            decimal sellingPrice = 0;
            if (!row.Cell(Col("SellingPrice")).TryGetValue(out sellingPrice) || sellingPrice < 0)
                rowErrors.Add("حقل SellingPrice مطلوب ويجب أن يكون رقماً موجباً.");

            int categoryId = 0;
            if (!row.Cell(Col("CategoryId")).TryGetValue(out categoryId))
                rowErrors.Add("حقل CategoryId مطلوب ويجب أن يكون رقماً صحيحاً.");
            else if (!validCategoryIds.Contains(categoryId))
                rowErrors.Add($"CategoryId ({categoryId}) غير موجود في قاعدة البيانات.");

            string? barcode = Col("Barcode") != -1 ? row.Cell(Col("Barcode")).GetString().Trim() : null;
            if (!string.IsNullOrWhiteSpace(barcode) && barcode.Length > 50)
                rowErrors.Add("حقل Barcode لا يجب أن يتجاوز 50 حرفاً.");

            string? nameAr = Col("NameAr") != -1 ? row.Cell(Col("NameAr")).GetString().Trim() : null;
            if (!string.IsNullOrWhiteSpace(nameAr) && nameAr.Length > 300)
                rowErrors.Add("حقل NameAr لا يجب أن يتجاوز 300 حرف.");

            string? description = Col("Description") != -1 ? row.Cell(Col("Description")).GetString().Trim() : null;

            decimal currentCostPrice = 0;
            var costCol = Col("CurrentCostPrice");
            if (costCol != -1 && !row.Cell(costCol).IsEmpty())
            {
                if (!row.Cell(costCol).TryGetValue(out currentCostPrice) || currentCostPrice < 0)
                    rowErrors.Add("حقل CurrentCostPrice يجب أن يكون رقماً موجباً.");
            }

            int quantityPerCarton = 1;
            var qtyCol = Col("QuantityPerCarton");
            if (qtyCol != -1 && !row.Cell(qtyCol).IsEmpty())
            {
                if (!row.Cell(qtyCol).TryGetValue(out int qty) || qty < 1)
                    rowErrors.Add("حقل QuantityPerCarton يجب أن يكون رقماً صحيحاً أكبر من صفر.");
                else
                    quantityPerCarton = qty;
            }

            // 👈 قراءة MinStockThreshold من الإكسيل وإلا وضع 0 افتراضياً
            int minStockThreshold = 0;
            var minStockCol = Col("MinStockThreshold");
            if (minStockCol != -1 && !row.Cell(minStockCol).IsEmpty())
            {
                if (row.Cell(minStockCol).TryGetValue(out int minVal) && minVal >= 0)
                    minStockThreshold = minVal;
            }

            bool isActive = ParseOptionalBool(row, Col("IsActive"), defaultValue: true);
            bool trackInventory = ParseOptionalBool(row, Col("TrackInventory"), defaultValue: true);

            int brandId = 1;
            var brandCol = Col("BrandId");
            if (brandCol != -1 && !row.Cell(brandCol).IsEmpty())
            {
                if (!row.Cell(brandCol).TryGetValue(out int bId))
                    rowErrors.Add("حقل BrandId يجب أن يكون رقماً صحيحاً.");
                else if (!validBrandIds.Contains(bId))
                    rowErrors.Add($"BrandId ({bId}) غير موجود في قاعدة البيانات.");
                else
                    brandId = bId;
            }

            var imageUrl = Col("ImageUrl") != -1 ? row.Cell(Col("ImageUrl")).GetString().Trim() : null;
            if (string.IsNullOrWhiteSpace(imageUrl))
                imageUrl = "/uploads/products/default.png";

            if (!string.IsNullOrWhiteSpace(sku))
            {
                if (existingSkuSet.Contains(sku))
                    rowErrors.Add($"SKU ({sku}) مستخدم بالفعل في قاعدة البيانات.");
                else if (!seenSkusInFile.Add(sku))
                    rowErrors.Add($"SKU ({sku}) مكرر أكثر من مرة داخل نفس ملف الاكسل.");
            }

            if (rowErrors.Count > 0)
            {
                result.Errors.Add(new BulkImportRowErrorDto
                {
                    RowNumber = rowNum,
                    SKU = sku,
                    Errors = rowErrors
                });
                continue;
            }

            productsToInsert.Add(new Product
            {
                SKU = sku,
                Barcode = string.IsNullOrWhiteSpace(barcode) ? null : barcode,
                Name = name,
                NameAr = string.IsNullOrWhiteSpace(nameAr) ? null : nameAr,
                Description = description,
                SellingPrice = sellingPrice,
                CurrentCostPrice = currentCostPrice,
                QuantityPerCarton = quantityPerCarton,
                MinStockThreshold = minStockThreshold, // 👈 تم إضافة الحقل هنا
                IsActive = isActive,
                TrackInventory = trackInventory,
                CategoryId = categoryId,
                BrandId = brandId,
                ImageUrl = imageUrl
            });

            result.ImportedSkus.Add(sku);
        }

        result.TotalRows = processedRows;
        result.FailedCount = result.Errors.Count;

        if (productsToInsert.Count > 0)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Products.AddRangeAsync(productsToInsert);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                result.SuccessCount = productsToInsert.Count;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new BadRequestException($"حدث خطأ أثناء حفظ المنتجات في قاعدة البيانات: {ex.Message}");
            }
        }

        return result;
    }

    private static bool ParseOptionalBool(IXLRow row, int colIndex, bool defaultValue)
    {
        if (colIndex == -1 || row.Cell(colIndex).IsEmpty())
            return defaultValue;

        var raw = row.Cell(colIndex).GetString().Trim();
        if (bool.TryParse(raw, out var parsed))
            return parsed;

        return raw == "1" || raw.Equals("true", StringComparison.OrdinalIgnoreCase) ||
               raw.Equals("نعم", StringComparison.OrdinalIgnoreCase);
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