namespace IbnAlZumar.API.DTOs.Catalog;

/// <summary>Returned by the manual "translate now" endpoints (ProductTranslationController).</summary>
public class TranslationResultDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? NameAr { get; set; }
    public bool IsAutoTranslated { get; set; }
    public bool TranslationApplied { get; set; }
    public string? Message { get; set; }
}
