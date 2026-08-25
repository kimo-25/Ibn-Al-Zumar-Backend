using System.Text.Json;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace IbnAlZumar.API.Ai.Tools
{
    public class GenerateProductsExcelTool : IAiTool
    {
        private const int MaxItems = 1000;

        public string Name => "generate_products_excel";

        public string Description =>
            "Generates a downloadable Excel (.xlsx) file listing extracted/prepared products, e.g. so the " +
            "user can review an invoice's contents before importing, or re-upload it via the existing " +
            "bulk-import screen. Returns a relative download URL.";

        public IReadOnlyCollection<string> AllowedRoles => AiRoles.CatalogWrite;

        public object ParametersSchema => new
        {
            type = "object",
            properties = new
            {
                fileName = new { type = "string", description = "Suggested file name without extension, e.g. 'invoice_2026_08_25' (optional)." },
                items = new
                {
                    type = "array",
                    items = new
                    {
                        type = "object",
                        properties = new
                        {
                            sku = new { type = "string" },
                            barcode = new { type = "string" },
                            name = new { type = "string" },
                            nameAr = new { type = "string" },
                            description = new { type = "string" },
                            sellingPrice = new { type = "number" },
                            currentCostPrice = new { type = "number" },
                            quantityPerCarton = new { type = "integer" },
                            categoryName = new { type = "string" }
                        },
                        required = new[] { "name" }
                    }
                }
            },
            required = new[] { "items" }
        };

        public Task<object> ExecuteAsync(JsonElement args, AiToolContext context, CancellationToken ct)
        {
            var items = args.GetArrayOrEmpty("items");
            if (items.Count == 0)
            {
                return Task.FromResult<object>(new { success = false, error = "لا توجد بيانات لإنشاء ملف اكسل." });
            }
            if (items.Count > MaxItems)
            {
                return Task.FromResult<object>(new { success = false, error = $"الحد الأقصى {MaxItems} صف لكل ملف." });
            }

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Products");

            string[] headers = { "SKU", "Barcode", "Name", "NameAr", "Description", "SellingPrice", "CurrentCostPrice", "QuantityPerCarton", "CategoryName" };
            for (var i = 0; i < headers.Length; i++)
            {
                ws.Cell(1, i + 1).Value = headers[i];
                ws.Cell(1, i + 1).Style.Font.Bold = true;
            }

            var row = 2;
            foreach (var item in items)
            {
                ws.Cell(row, 1).Value = item.GetStringOrNull("sku") ?? string.Empty;
                ws.Cell(row, 2).Value = item.GetStringOrNull("barcode") ?? string.Empty;
                ws.Cell(row, 3).Value = item.GetStringOrNull("name") ?? string.Empty;
                ws.Cell(row, 4).Value = item.GetStringOrNull("nameAr") ?? string.Empty;
                ws.Cell(row, 5).Value = item.GetStringOrNull("description") ?? string.Empty;
                ws.Cell(row, 6).Value = item.GetDecimalOrNull("sellingPrice") ?? 0;
                ws.Cell(row, 7).Value = item.GetDecimalOrNull("currentCostPrice") ?? 0;
                ws.Cell(row, 8).Value = item.GetIntOrNull("quantityPerCarton") ?? 1;
                ws.Cell(row, 9).Value = item.GetStringOrNull("categoryName") ?? string.Empty;
                row++;
            }

            ws.Columns().AdjustToContents();

            var env = context.Services.GetRequiredService<IWebHostEnvironment>();
            var rootPath = env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var exportsFolder = Path.Combine(rootPath, "uploads", "exports");
            Directory.CreateDirectory(exportsFolder);

            var requestedName = args.GetStringOrNull("fileName");
            var safeName = string.IsNullOrWhiteSpace(requestedName)
                ? "products_export"
                : string.Concat(requestedName.Split(Path.GetInvalidFileNameChars()));

            var fileName = $"{safeName}_{Guid.NewGuid().ToString("N")[..8]}.xlsx";
            var filePath = Path.Combine(exportsFolder, fileName);
            workbook.SaveAs(filePath);

            var downloadUrl = $"/uploads/exports/{fileName}";

            return Task.FromResult<object>(new
            {
                success = true,
                rowCount = items.Count,
                downloadUrl,
                fileName
            });
        }
    }
}