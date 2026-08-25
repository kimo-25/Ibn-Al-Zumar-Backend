using System.Text.Json;
using IbnAlZumar.API.Common.Exceptions;
using Services.Sales;

namespace IbnAlZumar.API.Ai.Tools
{
    /// <summary>Returns full details (items, totals, status, customer) for one order.</summary>
    public class GetOrderDetailsTool : IAiTool
    {
        public string Name => "get_order_details";

        public string Description =>
            "يرجع تفاصيل طلب محدد برقمه: المنتجات، الكميات، الإجمالي، والحالة. " +
            "Returns full details for one order by id: items, quantities, totals, and status.";

        public object ParametersSchema => new
        {
            type = "object",
            properties = new
            {
                orderId = new { type = "integer", description = "معرّف الطلب. The order id." }
            },
            required = new[] { "orderId" }
        };

        public IReadOnlyCollection<string> AllowedRoles => AiRoles.OperationalRead;

        public async Task<object> ExecuteAsync(JsonElement args, AiToolContext context, CancellationToken ct)
        {
            if (args.ValueKind != JsonValueKind.Object ||
                !args.TryGetProperty("orderId", out var idEl) || !idEl.TryGetInt32(out var orderId))
            {
                throw new ArgumentException("orderId is required.");
            }

            var orderService = context.Services.GetRequiredService<IOrderService>();

            try
            {
                // Staff calling the assistant always get the admin/moderator view —
                // ownership checks only apply to the customer-facing endpoint.
                var order = await orderService.GetOrderDetailsAsync(orderId, userEmail: null, isAdminOrMod: true);
                return order;
            }
            catch (NotFoundException)
            {
                return new { success = false, error = $"الطلب رقم {orderId} غير موجود." };
            }
        }
    }
}