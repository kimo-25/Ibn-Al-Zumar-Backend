using IbnAlZumar.API.DTOs.Ai;

namespace IbnAlZumar.API.Ai
{
    public interface IAiAssistantService
    {
        /// <summary>
        /// Runs one chat turn: sends the prompt + history to Gemini, executes any
        /// role-authorized tool calls it requests, and returns the final text reply.
        /// </summary>
        Task<AiChatResponseDto> ChatAsync(
            AiChatRequestDto request,
            string? userEmail,
            IReadOnlyCollection<string> userRoles,
            CancellationToken ct);
    }
}