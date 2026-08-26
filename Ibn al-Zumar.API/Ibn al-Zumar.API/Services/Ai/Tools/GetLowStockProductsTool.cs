using System.Text.Json;
using IbnAlZumar.API.Services.Inventory;

namespace IbnAlZumar.API.Ai.Tools
{
    /// <summary>Returns products whose quantity on hand is zero or at/below their reorder level.</summary>
    public class GetLowStockProductsTool : IAiTool
    {
        public string Name => "get_low_stock_products";

        public string Description =>
            "يرجع المنتجات منتهية المخزون (كميتها 0) أو التي وصل مخزونها لحد إعادة الطلب أو أقل. " +
            "Returns out of stock (quantity 0) and low stock products at or below their reorder level.";

        public object ParametersSchema => new
        {
            type = "object",
            properties = new
            {
                warehouseId = new
                {
                    type = "integer",
                    description = "معرّف المستودع (اختياري). Warehouse id to filter by. Omit to check all warehouses."
                }
            }
        };

        public IReadOnlyCollection<string> AllowedRoles => AiRoles.OperationalRead;

        public async Task<object> ExecuteAsync(JsonElement args, AiToolContext context, CancellationToken ct)
        {
            int? warehouseId = null;
            if (args.ValueKind == JsonValueKind.Object &&
                args.TryGetProperty("warehouseId", out var whEl) &&
                whEl.TryGetInt32(out var parsedWh))
            {
                warehouseId = parsedWh;
            }

            var inventoryService = context.Services.GetRequiredService<IInventoryService>();

            // استدعاء دالة النواقص المباشرة المعتمدة في الـ API Controller
            var lowStockProducts = await inventoryService.GetLowStockProductsAsync(warehouseId);
            var list = lowStockProducts.ToList();

            return new
            {
                totalCount = list.Count,
                summary = $"يوجد {list.Count} منتج منتهي أو أوشك على النفاد.",
                products = list.Take(50).Select(p => new
                {
                    Id = p.ProductId,
                    Sku = p.SKU,
                    Name = p.ProductName,
                    CurrentStock = p.QuantityOnHand,
                    MinStockThreshold = p.ReorderLevel,
                    IsOutOfStock = p.QuantityOnHand <= 0
                })
            };
        }
    }
}