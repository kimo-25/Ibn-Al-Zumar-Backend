using System.ComponentModel.DataAnnotations;

namespace IbnAlZumar.API.DTOs.Ai
{
    public class AiChatTurnDto
    {
        /// <summary>"user" | "assistant"</summary>
        [Required]
        public string Role { get; set; } = "user";

        [Required]
        public string Content { get; set; } = string.Empty;
    }
}