using IbnAlZumar.API.Services.Catalog;

namespace IbnAlZumar.API.Common.Helpers;

/// <summary>
/// Auto-translation helper (ARCHITECTURE.md §5.2c). Every bilingual entity stores a pair
/// (Product.Name/NameAr, Category.Name/NameAr, ...). Call this ONCE at create time, right
/// before SaveChangesAsync, from the owning service (e.g. ProductService.CreateAsync,
/// CategoriesService.CreateAsync) — never on update, so a human edit is never overwritten.
/// </summary>
public static class TranslationHelper
{
    /// <summary>
    /// Returns the (possibly-filled) pair and whether a translation was actually applied.
    /// If both sides are already present, or both are missing, nothing changes.
    /// If the provider fails (see ITranslationService), the original pair is returned
    /// untouched and AutoTranslated is false — a translation failure must never block
    /// the create.
    /// </summary>
    public static async Task<(string? Name, string? NameAr, bool AutoTranslated)> FillMissingSideAsync(
        ITranslationService translationService,
        string? name,
        string? nameAr,
        CancellationToken ct = default)
    {
        var hasName = !string.IsNullOrWhiteSpace(name);
        var hasNameAr = !string.IsNullOrWhiteSpace(nameAr);

        if (hasName == hasNameAr) return (name, nameAr, false); // both present or both empty

        if (hasName)
        {
            var translatedAr = await translationService.TranslateAsync(name!, "en", "ar", ct);
            return translatedAr is null ? (name, nameAr, false) : (name, translatedAr, true);
        }

        var translatedEn = await translationService.TranslateAsync(nameAr!, "ar", "en", ct);
        return translatedEn is null ? (name, nameAr, false) : (translatedEn, nameAr, true);
    }
}
