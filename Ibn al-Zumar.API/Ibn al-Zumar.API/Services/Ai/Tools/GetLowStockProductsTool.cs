using System.Text.Json;
using IbnAlZumar.API.Services.Inventory;

namespace IbnAlZumar.API.Ai.Tools
{
    /// <summary>Returns products whose quantity on hand is at/below their reorder level.</summary>
    public class GetLowStockProductsTool : IAiTool
    {
        public string Name => "get_low_stock_products";

        public string Description =>
            "يرجع المنتجات التي وصل مخزونها لحد إعادة الطلب أو أقل، اختيارياً في مستودع معيّن. " +
            "Returns products at or below their reorder level, optionally scoped to one warehouse.";

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
            var stockLevels = await inventoryService.GetStockLevelsAsync(warehouseId, search: null);

            var lowStock = stockLevels
                .Where(s => s.QuantityOnHand <= s.ReorderLevel)
                .OrderBy(s => s.QuantityOnHand)
                .Take(50)
                .ToList();

            return new
            {
                count = lowStock.Count,
                products = lowStock.Select(s => new
                {
                    s.ProductId,
                    s.SKU,
                    s.ProductName,
                    s.WarehouseId,
                    s.QuantityOnHand,
                    s.ReorderLevel
                })
            };
        }
    }
}