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

    [MaxLength(160)]
    public string? Slug { get; set; }

    public int? ParentCategoryId { get; set; }
}