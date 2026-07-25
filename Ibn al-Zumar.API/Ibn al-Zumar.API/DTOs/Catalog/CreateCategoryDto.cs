namespace IbnAlZumar.API.DTOs.Catalog;
using System.ComponentModel.DataAnnotations;


public class CreateCategoryDto
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? NameAr { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>Optional — if omitted, the service generates one from Name and guarantees uniqueness.</summary>
    [MaxLength(160)]
    public string? Slug { get; set; }

    /// <summary>Null = top-level category.</summary>
    public int? ParentCategoryId { get; set; }
}