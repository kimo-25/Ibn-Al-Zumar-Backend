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
        public GeminiContent? SystemInstruction { get; set; }

        [JsonPropertyName("contents")]
        public List<GeminiContent> Contents { get; set; } = new();

        [JsonPropertyName("tools")]
        public List<GeminiTool>? Tools { get; set; }

        [JsonPropertyName("toolConfig")]
        public GeminiToolConfig? ToolConfig { get; set; }

        [JsonPropertyName("generationConfig")]
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
        /// <summary>"user" | "model" | "function"</summary>
        [JsonPropertyName("role")]
        public string? Role { get; set; }

        [JsonPropertyName("parts")]
        public List<GeminiPart> Parts { get; set; } = new();
    }

    public class GeminiPart
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("functionCall")]
        public GeminiFunctionCall? FunctionCall { get; set; }

        [JsonPropertyName("functionResponse")]
        public GeminiFunctionResponse? FunctionResponse { get; set; }

        /// <summary>
        /// NEW — raw bytes (base64) for multimodal input: images (jpeg/png/webp) and PDFs.
        /// Gemini 1.5 reads these natively (OCR, layout, tables) without any server-side
        /// text extraction. Office formats (docx/xlsx) are NOT accepted here — those are
        /// extracted to plain text server-side and sent as a normal text part instead
        /// (see AiFileProcessingService).
        /// </summary>
        [JsonPropertyName("inlineData")]
        public GeminiInlineData? InlineData { get; set; }

        public static GeminiPart FromText(string text) => new() { Text = text };

        public static GeminiPart FromInlineData(string mimeType, string base64Data) =>
            new() { InlineData = new GeminiInlineData { MimeType = mimeType, Data = base64Data } };
    }

    public class GeminiInlineData
    {
        [JsonPropertyName("mimeType")]
        public string MimeType { get; set; } = string.Empty;

        /// <summary>Base64-encoded raw file bytes.</summary>
        [JsonPropertyName("data")]
        public string Data { get; set; } = string.Empty;
    }

    public class GeminiFunctionCall
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("args")]
        public JsonElement Args { get; set; }
    }

    public class GeminiFunctionResponse
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("response")]
        public object Response { get; set; } = new { };
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

        /// <summary>Raw OpenAPI-subset JSON schema object (see IAiTool.ParametersSchema).</summary>
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