using System.Text.Json;
using IbnAlZumar.API.Common.Exceptions;
using IbnAlZumar.API.DTOs.Catalog;
using IbnAlZumar.API.Services.Catalog;

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
                productId = new { type = "integer", description = "معرّف المنتج. The product id." },
                newPrice = new { type = "number", description = "السعر الجديد. The new selling price." }
            },
            required = new[] { "productId", "newPrice" }
        };

        public IReadOnlyCollection<string> AllowedRoles => AiRoles.SensitiveWrite;

        public async Task<object> ExecuteAsync(JsonElement args, AiToolContext context, CancellationToken ct)
        {
            if (!AiRoles.SensitiveWrite.Any(context.IsInRole))
            {
                throw new UnauthorizedAccessException("لا تملك صلاحية تعديل الأسعار.");
            }

            if (args.ValueKind != JsonValueKind.Object ||
                !args.TryGetProperty("productId", out var idEl) || !idEl.TryGetInt32(out var productId) ||
                !args.TryGetProperty("newPrice", out var priceEl) || !priceEl.TryGetDecimal(out var newPrice))
            {
                throw new ArgumentException("productId and newPrice are required.");
            }

            if (newPrice <= 0)
            {
                throw new ArgumentException("السعر يجب أن يكون أكبر من صفر.");
            }

            var productService = context.Services.GetRequiredService<IProductService>();

            try
            {
                var updateDto = new UpdateProductDto { CurrentCostPrice = newPrice };
                var updated = await productService.UpdateAsync(productId, updateDto);

                return new
                {
                    success = true,
                    productId = updated.Id,
                    newPrice = updated.CurrentCostPrice
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