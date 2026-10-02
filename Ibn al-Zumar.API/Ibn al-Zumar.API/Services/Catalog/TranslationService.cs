using System.Net.Http.Json;
using System.Text.Json;
using IbnAlZumar.API.Common.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IbnAlZumar.API.Services.Catalog;

/// <summary>
/// Talks to a LibreTranslate-compatible endpoint (self-hosted or the public instance) via the
/// typed HttpClient registered in Program.cs ("TranslationClient" / ITranslationService). Swap
/// the provider by changing Translation:BaseUrl (and, for a different API shape entirely,
/// replacing this class) — callers only depend on ITranslationService.
/// </summary>
public class TranslationService : ITranslationService
{
    private readonly HttpClient _httpClient;
    private readonly TranslationSettings _settings;
    private readonly ILogger<TranslationService> _logger;

    public TranslationService(HttpClient httpClient, IOptions<TranslationSettings> settings, ILogger<TranslationService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<string?> TranslateAsync(string text, string sourceLang, string targetLang, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(text)) return null;
        if (string.Equals(sourceLang, targetLang, StringComparison.OrdinalIgnoreCase)) return text;

        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(_settings.TimeoutSeconds));

            var payload = new
            {
                q = text,
                source = sourceLang,
                target = targetLang,
                format = "text",
                api_key = _settings.ApiKey
            };

            using var response = await _httpClient.PostAsJsonAsync("/translate", payload, cts.Token);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Translation provider returned {StatusCode} translating {Length} chars {Source}->{Target}",
                    response.StatusCode, text.Length, sourceLang, targetLang);
                return null;
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cts.Token);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cts.Token);

            return doc.RootElement.TryGetProperty("translatedText", out var translatedElement)
                ? translatedElement.GetString()
                : null;
        }
        catch (Exception ex)
        {
            // Best-effort per ARCHITECTURE.md §5.2c: a translation failure must never fail
            // the caller's request (e.g. product create/update). Log and move on.
            _logger.LogWarning(ex, "Translation request failed ({Source}->{Target}); continuing without translation", sourceLang, targetLang);
            return null;
        }
    }
}
