namespace IbnAlZumar.API.DTOs.Ai
{
    public class AiChatResponseDto
    {
        public string Reply { get; set; } = string.Empty;

        public List<string> ToolsUsed { get; set; } = new();

        /// <summary>
        /// NEW — if a tool produced a downloadable artifact this turn (e.g.
        /// generate_products_excel), its relative URL is surfaced here so the frontend
        /// can render a download link/button under the assistant's reply.
        /// </summary>
        public string? DownloadUrl { get; set; }

        public string? DownloadFileName { get; set; }
    }
}