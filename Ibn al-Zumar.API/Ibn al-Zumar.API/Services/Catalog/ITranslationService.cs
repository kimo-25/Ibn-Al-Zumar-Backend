namespace IbnAlZumar.API.Services.Catalog;

/// <summary>
/// Auto-translation helper (ARCHITECTURE.md §5.2c). A provider failure must NEVER fail the
/// caller's request — implementations return null instead of throwing on any error.
/// </summary>
public interface ITranslationService
{
    Task<string?> TranslateAsync(string text, string sourceLang, string targetLang, CancellationToken ct = default);
}
