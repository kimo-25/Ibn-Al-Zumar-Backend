using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using IbnAlZumar.Api.Services.Sales;
using IbnAlZumar.API.DTOs.Sales;
using IbnAlZumar.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

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

            // إرسال الـ Filter والـ CancellationToken لتوافق الـ Interface الجديد
            var filter = new OrderFilterDto
            {
                PageNumber = 1,
                PageSize = 100
            };

            var pagedResult = await orderService.GetAllOrdersAsync(filter, ct);

            var pendingStatuses = new[]
            {
                OrderStatus.PendingConfirmation.ToString(),
                OrderStatus.Processing.ToString(),
                OrderStatus.CancellationRequested.ToString()
            };

            var filtered = pagedResult.Items
                .Where(o => pendingStatuses.Contains(o.Status))
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