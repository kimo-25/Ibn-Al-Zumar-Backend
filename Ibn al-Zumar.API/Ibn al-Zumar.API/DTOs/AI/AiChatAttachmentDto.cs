using System.ComponentModel.DataAnnotations;

namespace IbnAlZumar.API.DTOs.Ai
{
    /// <summary>
    /// One uploaded file (invoice/document/image) attached to a chat turn, already
    /// converted to base64 by AiController before it reaches AiAssistantService.
    /// </summary>
    public class AiChatAttachmentDto
    {
        [Required, MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string MimeType { get; set; } = string.Empty;

        /// <summary>Base64-encoded raw file bytes.</summary>
        [Required]
        public string Base64Data { get; set; } = string.Empty;

        public long SizeBytes { get; set; }
    }
}