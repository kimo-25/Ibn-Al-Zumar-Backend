using IbnAlZumar.API.Ai.Models;

namespace IbnAlZumar.API.Ai.Tools
{
    public class AiToolRegistry
    {
        private readonly Dictionary<string, IAiTool> _tools;

        public AiToolRegistry(IEnumerable<IAiTool> tools)
        {
            _tools = tools.ToDictionary(t => t.Name, StringComparer.OrdinalIgnoreCase);
        }

        public IAiTool? FindAuthorized(string toolName, IReadOnlyCollection<string> userRoles)
        {
            if (!_tools.TryGetValue(toolName, out var tool))
            {
                return null;
            }

            var isAuthorized = tool.AllowedRoles.Any(role =>
                userRoles.Contains(role, StringComparer.OrdinalIgnoreCase));

            return isAuthorized ? tool : null;
        }

        public List<GeminiTool> BuildGeminiTools(IReadOnlyCollection<string> userRoles)
        {
            var authorizedTools = _tools.Values
                .Where(t => t.AllowedRoles.Any(role => userRoles.Contains(role, StringComparer.OrdinalIgnoreCase)))
                .Select(t => new GeminiFunctionDeclaration
                {
                    Name = t.Name,
                    Description = t.Description,
                    Parameters = t.ParametersSchema
                })
                .ToList();

            if (authorizedTools.Count == 0)
            {
                return new List<GeminiTool>();
            }

            return new List<GeminiTool>
            {
                new GeminiTool
                {
                    FunctionDeclarations = authorizedTools
                }
            };
        }
    }
}