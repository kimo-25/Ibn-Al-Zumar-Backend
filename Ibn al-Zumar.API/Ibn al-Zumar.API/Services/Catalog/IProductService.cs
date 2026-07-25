using IbnAlZumar.API.DTOs.Catalog;

namespace IbnAlZumar.API.Services.Catalog
{
    public interface IProductService
    {
        Task<PagedResultDto<ProductResponseDto>> GetAllAsync(ProductFilterDto filter);
        Task<ProductResponseDto> GetByIdAsync(int id);
        Task<ProductResponseDto> CreateAsync(CreateProductDto dto);
        Task<ProductResponseDto> UpdateAsync(int id, UpdateProductDto dto);
        Task DeleteAsync(int id);
    }
}