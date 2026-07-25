using IbnAlZumar.API.DTOs.Catalog; 
using IbnAlZumar.Api.Common.Helpers;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;

namespace IbnAlZumar.Api.Services.Catalog;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync()
    {
        return await _context.Set<Category>()
            .Include(c => c.ParentCategory)
            .Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                NameAr = c.NameAr,
                Description = c.Description,
                Slug = c.Slug,
                ParentCategoryId = c.ParentCategoryId,
                ParentCategoryName = c.ParentCategory != null ? c.ParentCategory.Name : null
            })
            .ToListAsync();
    }

    public async Task<CategoryResponseDto?> GetByIdAsync(int id)
    {
        return await _context.Set<Category>()
            .Include(c => c.ParentCategory)
            .Where(c => c.Id == id)
            .Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                NameAr = c.NameAr,
                Description = c.Description,
                Slug = c.Slug,
                ParentCategoryId = c.ParentCategoryId,
                ParentCategoryName = c.ParentCategory != null ? c.ParentCategory.Name : null
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto)
    {
        var slug = string.IsNullOrWhiteSpace(dto.Slug)
            ? SlugHelper.GenerateSlug(dto.Name)
            : SlugHelper.GenerateSlug(dto.Slug);

        var category = new Category
        {
            Name = dto.Name,
            NameAr = dto.NameAr,
            Description = dto.Description,
            Slug = slug,
            ParentCategoryId = dto.ParentCategoryId
        };

        _context.Set<Category>().Add(category);
        await _context.SaveChangesAsync();

        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            NameAr = category.NameAr,
            Description = category.Description,
            Slug = category.Slug,
            ParentCategoryId = category.ParentCategoryId
        };
    }

    public async Task<bool> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        var category = await _context.Set<Category>().FindAsync(id);
        if (category == null) return false;

        category.Name = dto.Name;
        category.NameAr = dto.NameAr;
        category.Description = dto.Description;
        category.Slug = string.IsNullOrWhiteSpace(dto.Slug)
            ? SlugHelper.GenerateSlug(dto.Name)
            : SlugHelper.GenerateSlug(dto.Slug);
        category.ParentCategoryId = dto.ParentCategoryId;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _context.Set<Category>().FindAsync(id);
        if (category == null) return false;

        _context.Set<Category>().Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }
}