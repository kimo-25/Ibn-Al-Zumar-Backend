using IbnAlZumar.API.Ai.Models;
using IbnAlZumar.API.DTOs.Ai;

namespace IbnAlZumar.API.Ai.Files
{
    public interface IAiFileProcessingService
    {
        /// <summary>
        /// Converts one uploaded attachment into a GeminiPart:
        /// - images (jpeg/png/webp) and PDFs are passed through as raw inlineData bytes —
        ///   Gemini 1.5's native vision handles OCR/layout/tables itself.
        /// - docx/xlsx are NOT accepted as inlineData by the Gemini API, so their text/
        ///   tabular content is extracted server-side and returned as a text part instead.
        /// </summary>
        Task<GeminiPart> BuildGeminiPartAsync(AiChatAttachmentDto attachment, CancellationToken ct);

        bool IsSupportedMimeType(string mimeType);
    }
}