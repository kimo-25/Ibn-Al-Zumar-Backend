using System.Text.Json;
using System.Text.Json.Serialization;
using ClosedXML.Excel;
using IbnAlZumar.Api.Common.Settings;
using IbnAlZumar.API.Ai;
using IbnAlZumar.API.Ai.Files;
using IbnAlZumar.API.Ai.Models;
using IbnAlZumar.API.DTOs.Ai;
using Microsoft.Extensions.Options;

namespace IbnAlZumar.API.Services.Catalog;

public sealed class InvoiceToExcelService : IInvoiceToExcelService
{
    private static readonly HashSet<string> AllowedMimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "image/jpeg", "image/jpg", "image/png", "image/webp", "image/heic", "image/heif"
    };
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly GeminiSettings _settings;
    private readonly IAiFileProcessingService _fileProcessing;
    private readonly ILogger<InvoiceToExcelService> _logger;

    public InvoiceToExcelService(IHttpClientFactory httpClientFactory, IOptions<GeminiSettings> settings, IAiFileProcessingService fileProcessing, ILogger<InvoiceToExcelService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _settings = settings.Value;
        _fileProcessing = fileProcessing;
        _logger = logger;
    }

    public async Task<InvoiceExcelFile> ConvertAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file.Length == 0 || file.Length > 20 * 1024 * 1024) throw new ArgumentException("File is empty or exceeds the 20 MB limit.");
        var mime = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType;
        if (!AllowedMimeTypes.Contains(mime)) throw new ArgumentException("Supported files are PDF, Word, image, and invoice documents.");
        await using var input = new MemoryStream();
        await file.CopyToAsync(input, cancellationToken);
        var attachment = new AiChatAttachmentDto { FileName = Path.GetFileName(file.FileName), MimeType = mime, SizeBytes = file.Length, Base64Data = Convert.ToBase64String(input.ToArray()) };
        var part = await _fileProcessing.BuildGeminiPartAsync(attachment, cancellationToken);
        var prompt = "Extract every product row from this invoice/document. Return ONLY a valid JSON array, no markdown. " +
                     "Each object must use exactly these keys: sku, name, nameAr, sellingPrice, quantityPerCarton, categoryId, brandId, minStockThreshold, description. " +
                     "Use null for missing values; never invent SKU, prices, IDs, or quantities. Numbers must be JSON numbers.";
        var request = new GeminiRequest
        {
            Contents = new List<GeminiContent> { new() { Role = "user", Parts = new List<GeminiPart> { GeminiPart.FromText(prompt), part } } },
            GenerationConfig = new GeminiGenerationConfig { Temperature = 0.1, MaxOutputTokens = 8192 }
        };
        var model = _settings.Model.Trim().Replace("models/", string.Empty, StringComparison.OrdinalIgnoreCase);
        var url = $"{_settings.BaseUrl.TrimEnd('/')}/models/{model}:generateContent?key={_settings.ApiKey}";
        using var response = await _httpClientFactory.CreateClient().PostAsJsonAsync(url, request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode) throw new InvalidOperationException("Gemini extraction failed.");
        var gemini = JsonSerializer.Deserialize<GeminiResponse>(body, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        var text = string.Concat(gemini?.Candidates?.FirstOrDefault()?.Content?.Parts.Where(p => p.Text != null).Select(p => p.Text) ?? Array.Empty<string>());
        var rows = ParseRows(text);
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Products");
        var headers = new[] { "SKU", "Name", "NameAr", "SellingPrice", "QuantityPerCarton", "CategoryId", "BrandId", "MinStockThreshold", "Description" };
        for (var i = 0; i < headers.Length; i++) { var cell = sheet.Cell(1, i + 1); cell.Value = headers[i]; cell.Style.Font.Bold = true; }
        for (var r = 0; r < rows.Count; r++)
        {
            var row = rows[r]; var values = new object?[] { row.Sku, row.Name, row.NameAr, row.SellingPrice, row.QuantityPerCarton, row.CategoryId, row.BrandId, row.MinStockThreshold, row.Description };
            for (var c = 0; c < values.Length; c++) if (values[c] != null) sheet.Cell(r + 2, c + 1).Value = XLCellValue.FromObject(values[c]);
        }
        sheet.SheetView.FreezeRows(1); sheet.Columns().AdjustToContents();
        await using var output = new MemoryStream(); workbook.SaveAs(output);
        return new InvoiceExcelFile(output.ToArray(), $"invoice_products_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx");
    }

    private static List<InvoiceRow> ParseRows(string text)
    {
        var start = text.IndexOf('['); var end = text.LastIndexOf(']');
        if (start < 0 || end <= start) throw new InvalidOperationException("Gemini returned no product rows.");
        return JsonSerializer.Deserialize<List<InvoiceRow>>(text[start..(end + 1)], new JsonSerializerOptions(JsonSerializerDefaults.Web)) ?? new List<InvoiceRow>();
    }

    private sealed class InvoiceRow
    {
        [JsonPropertyName("sku")] public string? Sku { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("nameAr")] public string? NameAr { get; set; }
        [JsonPropertyName("sellingPrice")] public decimal? SellingPrice { get; set; }
        [JsonPropertyName("quantityPerCarton")] public int? QuantityPerCarton { get; set; }
        [JsonPropertyName("categoryId")] public int? CategoryId { get; set; }
        [JsonPropertyName("brandId")] public int? BrandId { get; set; }
        [JsonPropertyName("minStockThreshold")] public int? MinStockThreshold { get; set; }
        [JsonPropertyName("description")] public string? Description { get; set; }
    }
}