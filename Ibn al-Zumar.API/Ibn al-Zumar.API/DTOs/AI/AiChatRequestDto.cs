using System.ComponentModel.DataAnnotations;

namespace IbnAlZumar.API.DTOs.Ai
{
    public class AiChatRequestDto
    {
        [Required, MaxLength(4000)]
        public string Prompt { get; set; } = string.Empty;

        public List<AiChatTurnDto> History { get; set; } = new();

        /// <summary>
        /// NEW — files attached to THIS turn (invoices, supplier lists, stock photos, PDFs,
        /// Word/Excel docs). Populated by AiController from the multipart upload; empty for
        /// plain text-only turns.
        /// </summary>
        public List<AiChatAttachmentDto> Attachments { get; set; } = new();
    }
}