using System.Text.Json;
using IbnAlZumar.Api.Services.Catalog;
using IbnAlZumar.API.DTOs.Catalog;
using IbnAlZumar.API.Services.Catalog;
using Microsoft.Extensions.DependencyInjection;

namespace IbnAlZumar.API.Ai.Tools
{
    public class CreateProductTool : IAiTool
    {
        public string Name => "create_product";

        public string Description =>
            "Creates a single product. Provide EITHER categoryId (preferred, from get_categories) OR " +
            "categoryName — if categoryName has no existing match, this tool will automatically create " +
            "that category first. Never invent a SKU or price that isn't in the source text/invoice; ask " +
            "the user for missing required fields instead. Per the system rules, restate what you're about " +
            "to create and get the user's confirmation before calling this tool, unless already confirmed.";

        public IReadOnlyCollection<string> AllowedRoles => AiRoles.CatalogWrite;

        public object ParametersSchema => new
        {
            type = "object",
            properties = new
            {
                sku = new { type = "string", description = "Unique product SKU (required)." },
                barcode = new { type = "string", description = "Barcode, if known (optional)." },
                name = new { type = "string", description = "Product name in English/Latin (required)." },
                nameAr = new { type = "string", description = "Product name in Arabic (optional)." },
                description = new { type = "string", description = "Description (optional)." },
                sellingPrice = new { type = "number", description = "Selling price (required)." },
                currentCostPrice = new { type = "number", description = "Cost price, if known (optional)." },
                quantityPerCarton = new { type = "integer", description = "Units per carton, default 1 (optional)." },
                categoryId = new { type = "integer", description = "Existing category id (preferred over categoryName)." },
                categoryName = new { type = "string", description = "Category name to resolve or auto-create if categoryId is not known." },
                isActive = new { type = "boolean", description = "Whether the product is active. Defaults to true." },
                trackInventory = new { type = "boolean", description = "Whether stock is tracked. Defaults to true." }
            },
            required = new[] { "sku", "name", "sellingPrice" }
        };

        public async Task<object> ExecuteAsync(JsonElement args, AiToolContext context, CancellationToken ct)
        {
            var sku = args.GetStringOrNull("sku");
            var name = args.GetStringOrNull("name");
            var sellingPrice = args.GetDecimalOrNull("sellingPrice");

            if (string.IsNullOrWhiteSpace(sku) || string.IsNullOrWhiteSpace(name) || sellingPrice == null)
            {
                return new { success = false, error = "الحقول المطلوبة: SKU، الاسم، وسعر البيع." };
            }

            var productService = context.Services.GetRequiredService<IProductService>();
            var categoryService = context.Services.GetRequiredService<ICategoryService>();

            var categoryId = args.GetIntOrNull("categoryId");
            var categoryName = args.GetStringOrNull("categoryName");

            if (categoryId == null)
            {
                if (string.IsNullOrWhiteSpace(categoryName))
                {
                    return new { success = false, error = "يجب تحديد التصنيف (categoryId أو categoryName)." };
                }

                var resolved = await ResolveOrCreateCategoryAsync(categoryService, categoryName);
                categoryId = resolved;
            }

            var dto = new CreateProductDto
            {
                SKU = sku,
                Barcode = args.GetStringOrNull("barcode"),
                Name = name,
                NameAr = args.GetStringOrNull("nameAr"),
                Description = args.GetStringOrNull("description"),
                SellingPrice = sellingPrice.Value,
                CurrentCostPrice = args.GetDecimalOrNull("currentCostPrice"),
                QuantityPerCarton = args.GetIntOrNull("quantityPerCarton") ?? 1,
                CategoryId = categoryId.Value,
                IsActive = args.GetBoolOrDefault("isActive", true),
                TrackInventory = args.GetBoolOrDefault("trackInventory", true)
            };

            try
            {
                var created = await productService.CreateAsync(dto);
                return new
                {
                    success = true,
                    product = new { created.Id, created.SKU, created.Name, created.SellingPrice }
                };
            }
            catch (Exception ex)
            {
                return new { success = false, error = $"تعذر إنشاء المنتج '{sku}': {ex.Message}" };
            }
        }

        internal static async Task<int> ResolveOrCreateCategoryAsync(ICategoryService categoryService, string categoryName)
        {
            var categories = await categoryService.GetAllAsync();
            var match = categories.FirstOrDefault(c =>
                string.Equals(c.Name, categoryName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(c.NameAr, categoryName, StringComparison.OrdinalIgnoreCase));

            if (match != null) return match.Id;

            var created = await categoryService.CreateAsync(new CreateCategoryDto { Name = categoryName });
            return created.Id;
        }
    }
}