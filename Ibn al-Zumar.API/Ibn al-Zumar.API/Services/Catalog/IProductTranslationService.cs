namespace IbnAlZumar.API.Services.Catalog;

using IbnAlZumar.API.DTOs.Catalog;

/// <summary>
/// Manual re-translate trigger for the admin/moderator product &amp; category forms
/// (AutoTranslatedBadge.jsx on the frontend). Distinct from TranslationHelper
/// (create-time, fills the missing side only, never overwrites) — this can also overwrite
/// an already-filled side, but ONLY when the user explicitly asks for it via `overwrite: true`.
/// </summary>
public interface IProductTranslationService
{
    Task<TranslationResultDto> TranslateProductAsync(int productId, bool overwrite, CancellationToken ct = default);

    Task<TranslationResultDto> TranslateCategoryAsync(int categoryId, bool overwrite, CancellationToken ct = default);
}
