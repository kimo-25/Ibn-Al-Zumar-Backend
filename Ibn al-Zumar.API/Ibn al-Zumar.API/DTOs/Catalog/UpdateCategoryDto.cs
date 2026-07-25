// File: DTOs/Catalog/UpdateCategoryDto.cs
using System.ComponentModel.DataAnnotations;

namespace IbnAlZumar.API.DTOs.Catalog;
/// <summary>Full-update (PUT) shape — the client sends the complete desired state of the category.</summary>
public class UpdateCategoryDto
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? NameAr { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(160)]
    public string? Slug { get; set; }

    public int? ParentCategoryId { get; set; }
}