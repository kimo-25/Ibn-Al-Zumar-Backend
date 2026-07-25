// File: DTOs/Catalog/CategoryResponseDto.cs
namespace IbnAlZumar.API.DTOs.Catalog;
public class CategoryResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? Description { get; set; }
    public string Slug { get; set; } = string.Empty;

    public int? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }

    /// <summary>Kept as counts (not full nested objects) so listing endpoints stay cheap.
    /// Use GET /api/categories?parentId={id} to fetch the actual subcategories.</summary>
    public int SubCategoryCount { get; set; }
    public int ProductCount { get; set; }

    public DateTime CreatedAt { get; set; }
}