using IbnAlZumar.API.Common.Exceptions;
using IbnAlZumar.API.DTOs.Catalog;
using IbnAlZumar.API.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IbnAlZumar.API.Services.Catalog;

public class ProductTranslationService : IProductTranslationService
{
    private readonly ApplicationDbContext _context;
    private readonly ITranslationService _translationService;

    public ProductTranslationService(ApplicationDbContext context, ITranslationService translationService)
    {
        _context = context;
        _translationService = translationService;
    }

    public async Task<TranslationResultDto> TranslateProductAsync(int productId, bool overwrite, CancellationToken ct = default)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId, ct)
            ?? throw new NotFoundException("المنتج غير موجود.");

        var result = new TranslationResultDto
        {
            Id = product.Id,
            Name = product.Name,
            NameAr = product.NameAr,
            IsAutoTranslated = product.IsAutoTranslated
        };

        var needsAr = overwrite || string.IsNullOrWhiteSpace(product.NameAr);
        var needsEn = overwrite || string.IsNullOrWhiteSpace(product.Name);

        // Prefer filling Arabic from English first (Name is [Required] — always present);
        // only fall back to filling English from Arabic when Arabic is the side that already
        // has content and English still needs it (relevant mainly under `overwrite`).
        if (needsAr && !string.IsNullOrWhiteSpace(product.Name))
        {
            var translated = await _translationService.TranslateAsync(product.Name, "en", "ar", ct);
            if (translated is not null)
            {
                product.NameAr = translated;
                product.IsAutoTranslated = true;
                result.TranslationApplied = true;
            }
        }
        else if (needsEn && !string.IsNullOrWhiteSpace(product.NameAr))
        {
            var translated = await _translationService.TranslateAsync(product.NameAr, "ar", "en", ct);
            if (translated is not null)
            {
                product.Name = translated;
                product.IsAutoTranslated = true;
                result.TranslationApplied = true;
            }
        }

        if (result.TranslationApplied)
        {
            await _context.SaveChangesAsync(ct);
        }
        else
        {
            result.Message = "تعذر التواصل مع خدمة الترجمة، أو لا يوجد نص مصدر يمكن الترجمة منه.";
        }

        result.Name = product.Name;
        result.NameAr = product.NameAr;
        result.IsAutoTranslated = product.IsAutoTranslated;
        return result;
    }

    public async Task<TranslationResultDto> TranslateCategoryAsync(int categoryId, bool overwrite, CancellationToken ct = default)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId, ct)
            ?? throw new NotFoundException("التصنيف غير موجود.");

        var result = new TranslationResultDto
        {
            Id = category.Id,
            Name = category.Name,
            NameAr = category.NameAr,
            IsAutoTranslated = category.IsAutoTranslated
        };

        var needsAr = overwrite || string.IsNullOrWhiteSpace(category.NameAr);
        var needsEn = overwrite || string.IsNullOrWhiteSpace(category.Name);

        if (needsAr && !string.IsNullOrWhiteSpace(category.Name))
        {
            var translated = await _translationService.TranslateAsync(category.Name, "en", "ar", ct);
            if (translated is not null)
            {
                category.NameAr = translated;
                category.IsAutoTranslated = true;
                result.TranslationApplied = true;
            }
        }
        else if (needsEn && !string.IsNullOrWhiteSpace(category.NameAr))
        {
            var translated = await _translationService.TranslateAsync(category.NameAr, "ar", "en", ct);
            if (translated is not null)
            {
                category.Name = translated;
                category.IsAutoTranslated = true;
                result.TranslationApplied = true;
            }
        }

        if (result.TranslationApplied)
        {
            await _context.SaveChangesAsync(ct);
        }
        else
        {
            result.Message = "تعذر التواصل مع خدمة الترجمة، أو لا يوجد نص مصدر يمكن الترجمة منه.";
        }

        result.Name = category.Name;
        result.NameAr = category.NameAr;
        result.IsAutoTranslated = category.IsAutoTranslated;
        return result;
    }
}
