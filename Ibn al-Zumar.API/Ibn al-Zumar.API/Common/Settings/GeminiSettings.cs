namespace IbnAlZumar.Api.Common.Settings;

/// <summary>
/// Configuration for the Google Gemini API, bound from the "Gemini" section of appsettings.json.
/// </summary>
public class GeminiSettings
{
    public const string SectionName = "Gemini";

    public string ApiKey { get; set; } = string.Empty;

    /// <summary>Model id, e.g. "gemini-1.5-flash" or "gemini-1.5-pro".</summary>
    public string Model { get; set; } = "gemini-1.5-flash";

    public string BaseUrl { get; set; } = "https://generativelanguage.googleapis.com/v1beta";

    /// <summary>Safety cap on how many tool-call round trips a single chat turn may take.</summary>
    public int MaxToolCallIterations { get; set; } = 5;
}