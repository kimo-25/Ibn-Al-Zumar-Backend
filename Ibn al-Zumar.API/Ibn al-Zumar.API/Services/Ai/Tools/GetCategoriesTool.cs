using System.Text.Json;
using IbnAlZumar.Api.Services.Catalog;
using Microsoft.Extensions.DependencyInjection;

namespace IbnAlZumar.API.Ai.Tools
{
    public class GetCategoriesTool : IAiTool
    {
        public string Name => "get_categories";

        public string Description =>
            "Returns the full list of existing product categories (id, name, Arabic name, slug, parent). " +
            "ALWAYS call this before create_category (to avoid duplicates) and before create_product / " +
            "bulk_import_products when you only have a category name from an invoice and need its id.";

        // تم التعديل إلى IReadOnlyCollection<string> لتفادي CS0738
        public IReadOnlyCollection<string> AllowedRoles => AiRoles.OperationalRead;

        public object ParametersSchema => new
        {
            type = "object",
            properties = new { },
        };

        public async Task<object> ExecuteAsync(JsonElement args, AiToolContext context, CancellationToken ct)
        {
            var categoryService = context.Services.GetRequiredService<ICategoryService>();
            var categories = await categoryService.GetAllAsync();

            return new
            {
                success = true,
                categories = categories.Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.NameAr,
                    c.Slug,
                    c.ParentCategoryId,
                    c.ParentCategoryName
                })
            };
        }
    }
}