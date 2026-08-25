using System.Text.Json;
using IbnAlZumar.Api.Services.Catalog;
using IbnAlZumar.API.DTOs.Catalog;
using Microsoft.Extensions.DependencyInjection;

namespace IbnAlZumar.API.Ai.Tools
{
    public class CreateCategoryTool : IAiTool
    {
        public string Name => "create_category";

        public string Description =>
            "Creates a new product category. Call get_categories first to make sure it doesn't already " +
            "exist (case-insensitive match on name/nameAr) — if it exists, reuse its id instead of calling " +
            "this. Per the system rules, restate what you're about to create and get the user's confirmation " +
            "before calling this tool, unless they already confirmed in this message.";

        public IReadOnlyCollection<string> AllowedRoles => AiRoles.CatalogWrite;

        public object ParametersSchema => new
        {
            type = "object",
            properties = new
            {
                name = new { type = "string", description = "Category name in English/Latin (required)." },
                nameAr = new { type = "string", description = "Category name in Arabic (optional but recommended)." },
                description = new { type = "string", description = "Short description (optional)." },
                parentCategoryId = new { type = "integer", description = "Id of the parent category, if this is a subcategory (optional)." }
            },
            required = new[] { "name" }
        };

        public async Task<object> ExecuteAsync(JsonElement args, AiToolContext context, CancellationToken ct)
        {
            var name = args.GetStringOrNull("name");
            if (string.IsNullOrWhiteSpace(name))
            {
                return new { success = false, error = "اسم التصنيف مطلوب." };
            }

            var categoryService = context.Services.GetRequiredService<ICategoryService>();

            var existing = await categoryService.GetAllAsync();
            var duplicate = existing.FirstOrDefault(c =>
                string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase) ||
                (!string.IsNullOrWhiteSpace(c.NameAr) &&
                 string.Equals(c.NameAr, args.GetStringOrNull("nameAr"), StringComparison.OrdinalIgnoreCase)));

            if (duplicate != null)
            {
                return new
                {
                    success = true,
                    alreadyExisted = true,
                    category = new { duplicate.Id, duplicate.Name, duplicate.NameAr }
                };
            }

            var dto = new CreateCategoryDto
            {
                Name = name,
                NameAr = args.GetStringOrNull("nameAr"),
                Description = args.GetStringOrNull("description"),
                ParentCategoryId = args.GetIntOrNull("parentCategoryId")
            };

            var created = await categoryService.CreateAsync(dto);

            return new
            {
                success = true,
                alreadyExisted = false,
                category = new { created.Id, created.Name, created.NameAr, created.Slug }
            };
        }
    }
}