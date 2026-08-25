using System.Text.Json;
using System.Text.Json.Serialization;

namespace IbnAlZumar.API.Ai.Models
{
    // ============================================================
    // Request models
    // ============================================================

    public class GeminiRequest
    {
        [JsonPropertyName("systemInstruction")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public GeminiContent? SystemInstruction { get; set; }

        [JsonPropertyName("contents")]
        public List<GeminiContent> Contents { get; set; } = new();

        [JsonPropertyName("tools")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<GeminiTool>? Tools { get; set; }

        [JsonPropertyName("toolConfig")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public GeminiToolConfig? ToolConfig { get; set; }

        [JsonPropertyName("generationConfig")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public GeminiGenerationConfig? GenerationConfig { get; set; }
    }

    public class GeminiGenerationConfig
    {
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; } = 0.3;

        [JsonPropertyName("maxOutputTokens")]
        public int MaxOutputTokens { get; set; } = 1024;
    }

    public class GeminiToolConfig
    {
        [JsonPropertyName("functionCallingConfig")]
        public GeminiFunctionCallingConfig FunctionCallingConfig { get; set; } = new();
    }

    public class GeminiFunctionCallingConfig
    {
        /// <summary>AUTO | ANY | NONE</summary>
        [JsonPropertyName("mode")]
        public string Mode { get; set; } = "AUTO";
    }

    public class GeminiContent
    {
        /// <summary>"user" | "model"</summary>
        [JsonPropertyName("role")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Role { get; set; }

        [JsonPropertyName("parts")]
        public List<GeminiPart> Parts { get; set; } = new();
    }

    public class GeminiPart
    {
        [JsonPropertyName("text")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Text { get; set; }

        [JsonPropertyName("functionCall")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public GeminiFunctionCall? FunctionCall { get; set; }

        [JsonPropertyName("functionResponse")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public GeminiFunctionResponse? FunctionResponse { get; set; }

        [JsonPropertyName("inlineData")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public GeminiInlineData? InlineData { get; set; }

        /// <summary>
        /// Required by Gemini 2.x/3.x "thinking" models for multi-turn function calling.
        /// IMPORTANT: this lives on the Part itself, as a SIBLING of "functionCall" —
        /// NOT nested inside GeminiFunctionCall, and NEVER present on a functionResponse
        /// part. Gemini returns it attached to the Part that carries the functionCall;
        /// we must echo that exact Part (including this field) back unchanged when we
        /// send the following turn, or the API rejects the request with
        /// "Function call is missing a thought_signature...".
        /// JSON key is camelCase per the v1beta REST spec: "thoughtSignature".
        /// </summary>
        [JsonPropertyName("thoughtSignature")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ThoughtSignature { get; set; }

        public static GeminiPart FromText(string text) => new() { Text = text };

        public static GeminiPart FromInlineData(string mimeType, string base64Data) =>
            new() { InlineData = new GeminiInlineData { MimeType = mimeType, Data = base64Data } };
    }

    public class GeminiInlineData
    {
        [JsonPropertyName("mimeType")]
        public string MimeType { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public string Data { get; set; } = string.Empty;
    }

    public class GeminiFunctionCall
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("args")]
        public JsonElement Args { get; set; }

        // NOTE: thoughtSignature intentionally does NOT live here — see GeminiPart.ThoughtSignature.
        // Gemini's REST schema puts it as a sibling of "functionCall" on the Part, not inside it.
    }

    public class GeminiFunctionResponse
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("response")]
        public object Response { get; set; } = new { };

        // NOTE: no thoughtSignature here, ever. A functionResponse part must NOT carry one —
        // sending it there is exactly what produces:
        // 'Unknown name "thoughtSignature" at contents[...].parts[...].function_response'
    }

    public class GeminiTool
    {
        [JsonPropertyName("functionDeclarations")]
        public List<GeminiFunctionDeclaration> FunctionDeclarations { get; set; } = new();
    }

    public class GeminiFunctionDeclaration
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("parameters")]
        public object Parameters { get; set; } = new { };
    }

    // ============================================================
    // Response models
    // ============================================================

    public class GeminiResponse
    {
        [JsonPropertyName("candidates")]
        public List<GeminiCandidate>? Candidates { get; set; }

        [JsonPropertyName("promptFeedback")]
        public GeminiPromptFeedback? PromptFeedback { get; set; }
    }

    public class GeminiPromptFeedback
    {
        [JsonPropertyName("blockReason")]
        public string? BlockReason { get; set; }
    }

    public class GeminiCandidate
    {
        [JsonPropertyName("content")]
        public GeminiContent? Content { get; set; }

        [JsonPropertyName("finishReason")]
        public string? FinishReason { get; set; }
    }
}