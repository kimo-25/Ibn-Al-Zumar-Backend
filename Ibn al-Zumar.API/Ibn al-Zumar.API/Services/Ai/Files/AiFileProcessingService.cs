using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using IbnAlZumar.API.Ai.Models;
using IbnAlZumar.API.DTOs.Ai;

namespace IbnAlZumar.API.Ai.Files
{
    /// <summary>
    /// NOTE ON NUGET PACKAGES: this implementation needs two packages that your project may
    /// not have yet:
    ///   dotnet add package ClosedXML
    ///   dotnet add package DocumentFormat.OpenXml
    /// If your existing Excel bulk-import code (IProductService.BulkImportAsync) already
    /// depends on one of these (or on EPPlus/NPOI instead), prefer reusing that same library
    /// here for consistency and swap the xlsx-reading block accordingly.
    /// </summary>
    public class AiFileProcessingService : IAiFileProcessingService
    {
        private static readonly HashSet<string> ImageAndPdfMimeTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg", "image/jpg", "image/png", "image/webp", "image/heic", "image/heif",
            "application/pdf"
        };

        private const string DocxMimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
        private const string XlsxMimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private const string XlsMimeType = "application/vnd.ms-excel";

        private readonly ILogger<AiFileProcessingService> _logger;

        public AiFileProcessingService(ILogger<AiFileProcessingService> logger)
        {
            _logger = logger;
        }

        public bool IsSupportedMimeType(string mimeType) =>
            ImageAndPdfMimeTypes.Contains(mimeType) ||
            string.Equals(mimeType, DocxMimeType, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(mimeType, XlsxMimeType, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(mimeType, XlsMimeType, StringComparison.OrdinalIgnoreCase);

        public async Task<GeminiPart> BuildGeminiPartAsync(AiChatAttachmentDto attachment, CancellationToken ct)
        {
            if (ImageAndPdfMimeTypes.Contains(attachment.MimeType))
            {
                // Native multimodal path — Gemini reads pixels/PDF pages directly.
                return GeminiPart.FromInlineData(attachment.MimeType, attachment.Base64Data);
            }

            if (string.Equals(attachment.MimeType, DocxMimeType, StringComparison.OrdinalIgnoreCase))
            {
                var text = ExtractDocxText(attachment);
                return GeminiPart.FromText(WrapExtractedText(attachment.FileName, text));
            }

            if (string.Equals(attachment.MimeType, XlsxMimeType, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(attachment.MimeType, XlsMimeType, StringComparison.OrdinalIgnoreCase))
            {
                var text = ExtractXlsxText(attachment);
                return GeminiPart.FromText(WrapExtractedText(attachment.FileName, text));
            }

            _logger.LogWarning("AiFileProcessingService: unsupported attachment mime type '{Mime}'", attachment.MimeType);
            return GeminiPart.FromText($"[تنبيه: تعذر قراءة الملف المرفق '{attachment.FileName}' لأن صيغته غير مدعومة.]");
        }

        private static string WrapExtractedText(string fileName, string extractedText)
        {
            if (string.IsNullOrWhiteSpace(extractedText))
            {
                return $"[الملف المرفق '{fileName}' لا يحتوي على نص قابل للاستخراج أو كان فارغاً.]";
            }

            return
                $"--- محتوى مستخرج من الملف المرفق: {fileName} ---\n" +
                extractedText +
                "\n--- نهاية محتوى الملف ---";
        }

        private string ExtractDocxText(AiChatAttachmentDto attachment)
        {
            try
            {
                var bytes = Convert.FromBase64String(attachment.Base64Data);
                using var stream = new MemoryStream(bytes);
                using var doc = WordprocessingDocument.Open(stream, false);

                var body = doc.MainDocumentPart?.Document?.Body;
                if (body == null) return string.Empty;

                var lines = new List<string>();

                foreach (var element in body.Elements())
                {
                    if (element is Paragraph para)
                    {
                        var text = para.InnerText?.Trim();
                        if (!string.IsNullOrWhiteSpace(text)) lines.Add(text);
                    }
                    else if (element is Table table)
                    {
                        foreach (var row in table.Elements<TableRow>())
                        {
                            var cells = row.Elements<TableCell>().Select(c => c.InnerText?.Trim() ?? string.Empty);
                            lines.Add(string.Join(" | ", cells));
                        }
                    }
                }

                // Cap to keep the request payload reasonable.
                return string.Join("\n", lines.Take(500));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AiFileProcessingService: failed to extract text from docx '{File}'", attachment.FileName);
                return string.Empty;
            }
        }

        private string ExtractXlsxText(AiChatAttachmentDto attachment)
        {
            try
            {
                var bytes = Convert.FromBase64String(attachment.Base64Data);
                using var stream = new MemoryStream(bytes);
                using var workbook = new XLWorkbook(stream);

                var sb = new System.Text.StringBuilder();
                const int maxRowsPerSheet = 300;
                const int maxColsPerSheet = 30;

                foreach (var ws in workbook.Worksheets)
                {
                    sb.AppendLine($"[Sheet: {ws.Name}]");

                    var usedRange = ws.RangeUsed();
                    if (usedRange == null) continue;

                    var rowCount = Math.Min(usedRange.RowCount(), maxRowsPerSheet);
                    var colCount = Math.Min(usedRange.ColumnCount(), maxColsPerSheet);

                    for (var r = 1; r <= rowCount; r++)
                    {
                        var cells = new List<string>();
                        for (var c = 1; c <= colCount; c++)
                        {
                            var cell = usedRange.Cell(r, c);
                            cells.Add(cell.GetFormattedString());
                        }
                        sb.AppendLine(string.Join(" | ", cells));
                    }
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AiFileProcessingService: failed to extract text from xlsx '{File}'", attachment.FileName);
                return string.Empty;
            }
        }
    }
}