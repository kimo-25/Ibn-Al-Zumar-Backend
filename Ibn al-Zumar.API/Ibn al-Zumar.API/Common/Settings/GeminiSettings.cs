namespace IbnAlZumar.Api.Common.Settings;

/// <summary>
/// Configuration for the Google Gemini API, bound from the "Gemini" section of appsettings.json.
/// Supports Azure Environment Variables with double underscores (Gemini__ApiKey, Gemini__Model, etc.)
/// </summary>
public class GeminiSettings
{
    public const string SectionName = "Gemini";

    /// <summary>Google Gemini API Key. Can be set via Azure Env Var: Gemini__ApiKey</summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>Model id, e.g. "gemini-1.5-flash" or "gemini-2.0-flash". Default: gemini-1.5-flash. Can be set via Azure Env Var: Gemini__Model</summary>
    public string Model { get; set; } = "gemini-1.5-flash";

    /// <summary>Base URL for Gemini API. Default: https://generativelanguage.googleapis.com/v1beta. Can be set via Azure Env Var: Gemini__BaseUrl</summary>
    public string BaseUrl { get; set; } = "https://generativelanguage.googleapis.com/v1beta";

    /// <summary>Safety cap on how many tool-call round trips a single chat turn may take.</summary>
    public int MaxToolCallIterations { get; set; } = 5;
}