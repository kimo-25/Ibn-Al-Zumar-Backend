using System.Security.Claims;
using System.Text.Json;
using IbnAlZumar.API.Ai;
using IbnAlZumar.API.Ai.Tools;
using IbnAlZumar.API.DTOs.Ai;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IbnAlZumar.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // any authenticated staff member may reach the endpoint — per-tool RBAC happens inside AiAssistantService
    public class AiController : ControllerBase
    {
        private readonly IAiAssistantService _aiAssistantService;
        private readonly ILogger<AiController> _logger;

        private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB per file
        private const int MaxFilesPerMessage = 5;

        private static readonly HashSet<string> AllowedMimeTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg", "image/jpg", "image/png", "image/webp",
            "application/pdf",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document", // .docx
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",       // .xlsx
            "application/vnd.ms-excel"                                                  // .xls
        };

        public AiController(IAiAssistantService aiAssistantService, ILogger<AiController> logger)
        {
            _aiAssistantService = aiAssistantService;
            _logger = logger;
        }

        /// <summary>
        /// Sends a prompt (optionally with file attachments) to the AI assistant. The user's
        /// JWT role claims are read server-side and passed to the assistant — the client never
        /// gets to say "trust me, I'm an Admin". Accepts multipart/form-data so invoices/
        /// documents/images can ride along with the text prompt in a single request:
        ///   prompt        (string, required)
        ///   historyJson   (string, optional — JSON array of {role, content})
        ///   files         (0-5 files, optional — images/pdf/docx/xlsx, 10MB each max)
        /// </summary>
        [HttpPost("chat")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(60 * 1024 * 1024)] // headroom above MaxFileSizeBytes * MaxFilesPerMessage
        [ProducesResponseType(typeof(AiChatResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> Chat(
            [FromForm] string prompt,
            [FromForm] string? historyJson,
            [FromForm] List<IFormFile>? files,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                return BadRequest(new { message = "الرسالة فارغة." });
            }

            var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email");

            var userRoles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
                .Select(c => c.Value)
                .Distinct()
                .ToList();

            if (userRoles.Count == 0)
            {
                return Forbid();
            }

            var attachments = new List<AiChatAttachmentDto>();

            if (files is { Count: > 0 })
            {
                // Only roles trusted with catalog writes may feed documents to the assistant —
                // everyone else can still chat, just without file understanding.
                var canUpload = userRoles.Any(r => AiRoles.FileUpload.Contains(r, StringComparer.OrdinalIgnoreCase));
                if (!canUpload)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new { message = "رفع الملفات متاح فقط لمدير النظام أو المشرف." });
                }

                if (files.Count > MaxFilesPerMessage)
                {
                    return BadRequest(new { message = $"الحد الأقصى {MaxFilesPerMessage} ملفات في الرسالة الواحدة." });
                }

                foreach (var file in files)
                {
                    if (file.Length == 0) continue;

                    if (file.Length > MaxFileSizeBytes)
                    {
                        return BadRequest(new { message = $"الملف '{file.FileName}' أكبر من الحد المسموح (10 ميجابايت)." });
                    }

                    var mimeType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType;
                    if (!AllowedMimeTypes.Contains(mimeType))
                    {
                        return BadRequest(new
                        {
                            message = $"صيغة الملف '{file.FileName}' غير مدعومة. الصيغ المسموحة: صور (JPG/PNG/WEBP)، PDF، Word (.docx)، Excel (.xlsx/.xls)."
                        });
                    }

                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms, ct);

                    attachments.Add(new AiChatAttachmentDto
                    {
                        FileName = file.FileName,
                        MimeType = mimeType,
                        SizeBytes = file.Length,
                        Base64Data = Convert.ToBase64String(ms.ToArray())
                    });
                }
            }

            var history = new List<AiChatTurnDto>();
            if (!string.IsNullOrWhiteSpace(historyJson))
            {
                try
                {
                    history = JsonSerializer.Deserialize<List<AiChatTurnDto>>(
                        historyJson, new JsonSerializerOptions(JsonSerializerDefaults.Web)) ?? new List<AiChatTurnDto>();
                }
                catch (JsonException)
                {
                    return BadRequest(new { message = "سجل المحادثة غير صالح." });
                }
            }

            var request = new AiChatRequestDto
            {
                Prompt = prompt,
                History = history,
                Attachments = attachments
            };

            try
            {
                var result = await _aiAssistantService.ChatAsync(request, userEmail, userRoles, ct);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "AI assistant call failed");
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new { message = "المساعد الذكي غير متاح حالياً، حاول لاحقاً." });
            }
        }
    }
}