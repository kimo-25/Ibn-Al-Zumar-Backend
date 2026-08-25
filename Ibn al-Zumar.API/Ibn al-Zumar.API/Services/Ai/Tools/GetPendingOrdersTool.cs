using System.Text.Json;
using IbnAlZumar.Domain.Enums;
using Services.Sales;

namespace IbnAlZumar.API.Ai.Tools
{
    /// <summary>Returns orders awaiting confirmation/processing so staff can triage the queue.</summary>
    public class GetPendingOrdersTool : IAiTool
    {
        public string Name => "get_pending_orders";

        public string Description =>
            "يرجع قائمة بالطلبات التي لم تُعالج بعد (قيد التأكيد أو قيد المعالجة أو طلبات إلغاء بانتظار المراجعة). " +
            "Returns orders that still need staff action: PendingConfirmation, Processing, or CancellationRequested.";

        public object ParametersSchema => new
        {
            type = "object",
            properties = new
            {
                limit = new
                {
                    type = "integer",
                    description = "أقصى عدد من الطلبات المطلوب إرجاعها. Max number of orders to return. Defaults to 20."
                }
            }
        };

        public IReadOnlyCollection<string> AllowedRoles => AiRoles.OperationalRead;

        public async Task<object> ExecuteAsync(JsonElement args, AiToolContext context, CancellationToken ct)
        {
            var limit = 20;
            if (args.ValueKind == JsonValueKind.Object &&
                args.TryGetProperty("limit", out var limitEl) &&
                limitEl.TryGetInt32(out var parsedLimit) && parsedLimit > 0)
            {
                limit = Math.Min(parsedLimit, 100);
            }

            var orderService = context.Services.GetRequiredService<IOrderService>();
            var allOrders = await orderService.GetAllOrdersAsync();

            var pendingStatuses = new[]
            {
                OrderStatus.PendingConfirmation.ToString(),
                OrderStatus.Processing.ToString(),
                OrderStatus.CancellationRequested.ToString()
            };

            // GetAllOrdersAsync projects to an anonymous type; read the Status field
            // dynamically rather than adding a bespoke DTO just for the assistant.
            var filtered = allOrders
                .Where(o => pendingStatuses.Contains(((dynamic)o).Status as string))
                .Take(limit)
                .ToList();

            return new
            {
                count = filtered.Count,
                orders = filtered
            };
        }
    }
}