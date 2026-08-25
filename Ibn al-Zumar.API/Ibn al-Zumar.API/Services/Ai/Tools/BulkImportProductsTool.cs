using System.Text.Json;
using IbnAlZumar.Api.Services.Catalog;
using IbnAlZumar.API.DTOs.Catalog;
using IbnAlZumar.API.Services.Catalog;
using Microsoft.Extensions.DependencyInjection;

namespace IbnAlZumar.API.Ai.Tools
{
    public class BulkImportProductsTool : IAiTool
    {
        private const int MaxItemsPerCall = 200;

        public string Name => "bulk_import_products";

        public string Description =>
            "Creates multiple products at once from a list of extracted items (e.g. parsed from an " +
            "uploaded invoice, supplier list, or spreadsheet). Each item needs sku, name, and sellingPrice " +
            "at minimum; category can be given as categoryId or categoryName (auto-created if new). " +
            "Rows that fail validation are skipped and reported individually — the whole batch does not " +
            "abort on a single bad row. ALWAYS show the user the parsed list and get explicit confirmation " +
            "before calling this tool, per the system rules for write actions.";

        // تعديل نوع الـ Return Type ليتوافق مع الـ Interface
        public IReadOnlyCollection<string> AllowedRoles => AiRoles.CatalogWrite;

        public object ParametersSchema => new
        {
            type = "object",
            properties = new
            {
                items = new
                {
                    type = "array",
                    description = $"List of products to create (max {MaxItemsPerCall} per call).",
                    items = new
                    {
                        type = "object",
                        properties = new
                        {
                            sku = new { type = "string" },
                            barcode = new { type = "string" },
                            name = new { type = "string" },
                            nameAr = new { type = "string" },
                            description = new { type = "string" },
                            sellingPrice = new { type = "number" },
                            currentCostPrice = new { type = "number" },
                            quantityPerCarton = new { type = "integer" },
                            categoryId = new { type = "integer" },
                            categoryName = new { type = "string" }
                        },
                        required = new[] { "sku", "name", "sellingPrice" }
                    }
                }
            },
            required = new[] { "items" }
        };

        public async Task<object> ExecuteAsync(JsonElement args, AiToolContext context, CancellationToken ct)
        {
            var items = args.GetArrayOrEmpty("items");
            if (items.Count == 0)
            {
                return new { success = false, error = "لا توجد عناصر لاستيرادها." };
            }

            if (items.Count > MaxItemsPerCall)
            {
                return new { success = false, error = $"الحد الأقصى {MaxItemsPerCall} منتج في المرة الواحدة. قسّم القائمة إلى دفعات أصغر." };
            }

            var productService = context.Services.GetRequiredService<IProductService>();
            var categoryService = context.Services.GetRequiredService<ICategoryService>();

            var importedSkus = new List<string>();
            var errors = new List<object>();
            var rowNumber = 0;

            var resolvedCategoryIds = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in items)
            {
                rowNumber++;
                var sku = item.GetStringOrNull("sku");
                var name = item.GetStringOrNull("name");
                var sellingPrice = item.GetDecimalOrNull("sellingPrice");

                if (string.IsNullOrWhiteSpace(sku) || string.IsNullOrWhiteSpace(name) || sellingPrice == null)
                {
                    errors.Add(new { row = rowNumber, sku, error = "بيانات ناقصة (SKU/الاسم/السعر)." });
                    continue;
                }

                try
                {
                    var categoryId = item.GetIntOrNull("categoryId");
                    var categoryName = item.GetStringOrNull("categoryName");

                    if (categoryId == null)
                    {
                        if (string.IsNullOrWhiteSpace(categoryName))
                        {
                            errors.Add(new { row = rowNumber, sku, error = "التصنيف مفقود." });
                            continue;
                        }

                        if (!resolvedCategoryIds.TryGetValue(categoryName, out var cachedId))
                        {
                            cachedId = await CreateProductTool.ResolveOrCreateCategoryAsync(categoryService, categoryName);
                            resolvedCategoryIds[categoryName] = cachedId;
                        }
                        categoryId = cachedId;
                    }

                    var dto = new CreateProductDto
                    {
                        SKU = sku,
                        Barcode = item.GetStringOrNull("barcode"),
                        Name = name,
                        NameAr = item.GetStringOrNull("nameAr"),
                        Description = item.GetStringOrNull("description"),
                        SellingPrice = sellingPrice.Value,
                        CurrentCostPrice = item.GetDecimalOrNull("currentCostPrice"),
                        QuantityPerCarton = item.GetIntOrNull("quantityPerCarton") ?? 1,
                        CategoryId = categoryId.Value,
                        IsActive = true,
                        TrackInventory = true
                    };

                    await productService.CreateAsync(dto);
                    importedSkus.Add(sku);
                }
                catch (Exception ex)
                {
                    errors.Add(new { row = rowNumber, sku, error = ex.Message });
                }
            }

            return new
            {
                success = true,
                totalRows = items.Count,
                successCount = importedSkus.Count,
                failedCount = errors.Count,
                importedSkus,
                errors
            };
        }
    }
}