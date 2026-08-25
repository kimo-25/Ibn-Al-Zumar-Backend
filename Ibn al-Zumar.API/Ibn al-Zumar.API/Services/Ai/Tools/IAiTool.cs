using System.Text.Json;

namespace IbnAlZumar.API.Ai.Tools
{
    /// <summary>
    /// A single callable function exposed to Gemini's function-calling API.
    /// Each tool declares which roles may see/invoke it — this is the enforcement
    /// point for the RBAC boundary.
    /// </summary>
    public interface IAiTool
    {
        /// <summary>Must match the function name Gemini is told about — snake_case, stable.</summary>
        string Name { get; }

        /// <summary>Shown to Gemini so it knows when/how to call this tool.</summary>
        string Description { get; }

        /// <summary>
        /// OpenAPI-subset JSON schema object describing the function's parameters.
        /// </summary>
        object ParametersSchema { get; }

        /// <summary>
        /// Roles allowed to see and execute this tool.
        /// Returned as IReadOnlyCollection to support arrays (string[]) and lists seamlessly.
        /// </summary>
        IReadOnlyCollection<string> AllowedRoles { get; }

        /// <summary>
        /// Executes the tool logic.
        /// </summary>
        Task<object> ExecuteAsync(JsonElement args, AiToolContext context, CancellationToken ct);
    }

    /// <summary>
    /// Encapsulates user context and scoped service provider for tool execution.
    /// </summary>
    public class AiToolContext
    {
        public string? UserEmail { get; set; }
        public IReadOnlyCollection<string> Roles { get; set; } = Array.Empty<string>();
        public IServiceProvider Services { get; set; } = default!;

        public bool IsInRole(string role) =>
            Roles.Any(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase));
    }
}