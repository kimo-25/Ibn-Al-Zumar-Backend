using System.Text.Json;
using IbnAlZumar.API.Common.Exceptions;
using IbnAlZumar.API.DTOs.Catalog;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.API.Services.Catalog;
using Microsoft.EntityFrameworkCore;

namespace IbnAlZumar.API.Ai.Tools
{
    /// <summary>
    /// Sensitive write action: changes a live selling price. Restricted to Admin —
    /// a Moderator/Cashier prompt asking for this must never reach ExecuteAsync,
    /// because the registry won't offer this tool to Gemini for their role at all.
    /// </summary>
    public class UpdateProductPriceTool : IAiTool
    {
        public string Name => "update_product_price";

        public string Description =>
            "يقوم بتحديث سعر بيع منتج معيّن. إجراء كتابة حساس، للإدمن فقط، ولا يتم تنفيذه إلا بعد تأكيد صريح من المستخدم في المحادثة. " +
            "Updates a product's selling price. Sensitive write action, Admin only — only call this after the user has explicitly confirmed the change in the conversation.";

        public object ParametersSchema => new
        {
            type = "object",
            properties = new
            {
                productId = new { type = "integer", description = "معرّف المنتج إن توفر. The product id if known." },
                sku = new { type = "string", description = "SKU المنتج. Product SKU." },
                productName = new { type = "string", description = "اسم المنتج بالعربية أو الإنجليزية. Product name." },
                newPrice = new { type = "number", description = "السعر الجديد. The new selling price." }
            },
            required = new[] { "newPrice" }
        };

        public IReadOnlyCollection<string> AllowedRoles => AiRoles.SensitiveWrite;

        public async Task<object> ExecuteAsync(JsonElement args, AiToolContext context, CancellationToken ct)
        {
            if (!AiRoles.SensitiveWrite.Any(context.IsInRole))
            {
                throw new UnauthorizedAccessException("لا تملك صلاحية تعديل الأسعار.");
            }

            if (args.ValueKind != JsonValueKind.Object ||
                !args.TryGetProperty("newPrice", out var priceEl) || !priceEl.TryGetDecimal(out var newPrice))
            {
                throw new ArgumentException("newPrice is required.");
            }

            var productId = 0;
            if (args.TryGetProperty("productId", out var idEl)) idEl.TryGetInt32(out productId);
            var sku = args.TryGetProperty("sku", out var skuEl) ? skuEl.GetString() : null;
            var productName = args.TryGetProperty("productName", out var nameEl) ? nameEl.GetString() : null;
            if (productId <= 0 && string.IsNullOrWhiteSpace(sku) && string.IsNullOrWhiteSpace(productName))
                throw new ArgumentException("productId, sku, or productName is required.");

            if (newPrice <= 0)
            {
                throw new ArgumentException("السعر يجب أن يكون أكبر من صفر.");
            }

            var productService = context.Services.GetRequiredService<IProductService>();
            if (productId <= 0)
            {
                var db = context.Services.GetRequiredService<ApplicationDbContext>();
                var query = db.Products.AsQueryable();
                if (!string.IsNullOrWhiteSpace(sku))
                    query = query.Where(p => p.SKU == sku);
                else
                    query = query.Where(p => p.Name == productName || p.NameAr == productName || p.Name.Contains(productName!) || (p.NameAr != null && p.NameAr.Contains(productName!)));
                productId = await query.Select(p => p.Id).FirstOrDefaultAsync(ct);
            }
            if (productId <= 0) throw new NotFoundException("لم يتم العثور على المنتج المطلوب.");

            try
            {
                var updated = await productService.UpdatePriceAsync(productId, newPrice);

                return new
                {
                    success = true,
                    productId = updated.Id,
                    newPrice = updated.SellingPrice
                };
            }
            catch (NotFoundException)
            {
                return new { success = false, error = $"المنتج رقم {productId} غير موجود." };
            }
            catch (BadRequestException ex)
            {
                return new { success = false, error = ex.Message };
            }
        }
    }
}