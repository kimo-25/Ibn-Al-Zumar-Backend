namespace IbnAlZumar.API.Common.Settings;

/// <summary>Bound from appsettings config section "Translation". See Program.cs wiring notes.</summary>
public class TranslationSettings
{
    public string Provider { get; set; } = "LibreTranslate";
    public string BaseUrl { get; set; } = "https://libretranslate.com";
    public string? ApiKey { get; set; }
    public int TimeoutSeconds { get; set; } = 8;
    public string DefaultSourceLanguage { get; set; } = "ar";
}
