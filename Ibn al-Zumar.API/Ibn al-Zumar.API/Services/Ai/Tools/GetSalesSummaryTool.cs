using System.Text.Json;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace IbnAlZumar.API.Ai.Tools
{
    /// <summary>
    /// Confidential: aggregate revenue/order-count numbers over a date range.
    /// Deliberately restricted to AiRoles.FinancialRead — a Moderator/Cashier
    /// asking for this must be refused before this tool is even offered to Gemini.
    /// </summary>
    public class GetSalesSummaryTool : IAiTool
    {
        public string Name => "get_sales_summary";

        public string Description =>
            "يرجع ملخص المبيعات (إجمالي الإيرادات وعدد الطلبات) خلال فترة زمنية محددة. بيانات مالية حساسة للإدارة فقط. " +
            "Returns confidential revenue totals and order counts for a date range. Admin/Owner only.";

        public object ParametersSchema => new
        {
            type = "object",
            properties = new
            {
                fromDate = new
                {
                    type = "string",
                    format = "date",
                    description = "تاريخ البداية بصيغة YYYY-MM-DD. Start date, inclusive."
                },
                toDate = new
                {
                    type = "string",
                    format = "date",
                    description = "تاريخ النهاية بصيغة YYYY-MM-DD. End date, inclusive. Defaults to today."
                }
            },
            required = new[] { "fromDate" }
        };

        public IReadOnlyCollection<string> AllowedRoles => AiRoles.FinancialRead;

        public async Task<object> ExecuteAsync(JsonElement args, AiToolContext context, CancellationToken ct)
        {
            // Defense in depth: even though the registry already filtered this tool out
            // for non-financial roles before Gemini ever saw it, re-check here in case
            // ExecuteAsync is ever invoked from a different call path.
            if (!AiRoles.FinancialRead.Any(context.IsInRole))
            {
                throw new UnauthorizedAccessException("هذه البيانات المالية غير متاحة لدورك الحالي.");
            }

            if (args.ValueKind != JsonValueKind.Object || !args.TryGetProperty("fromDate", out var fromEl) ||
                !DateTime.TryParse(fromEl.GetString(), out var fromDate))
            {
                throw new ArgumentException("fromDate is required and must be a valid date (YYYY-MM-DD).");
            }

            var toDate = DateTime.UtcNow.Date;
            if (args.TryGetProperty("toDate", out var toEl) && DateTime.TryParse(toEl.GetString(), out var parsedTo))
            {
                toDate = parsedTo.Date;
            }

            var toDateExclusive = toDate.AddDays(1);

            var dbContext = context.Services.GetRequiredService<ApplicationDbContext>();

            var query = dbContext.Orders
                .AsNoTracking()
                .Where(o => o.OrderDate >= fromDate.Date && o.OrderDate < toDateExclusive
                            && o.Status != OrderStatus.Cancelled);

            var orderCount = await query.CountAsync(ct);
            var totalRevenue = orderCount == 0 ? 0m : await query.SumAsync(o => o.TotalAmount, ct);

            return new
            {
                fromDate = fromDate.ToString("yyyy-MM-dd"),
                toDate = toDate.ToString("yyyy-MM-dd"),
                orderCount,
                totalRevenue
            };
        }
    }
}