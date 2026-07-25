using IbnAlZumar.API.DTOs.Catalog;
using IbnAlZumar.API.DTOs.Customers;

namespace IbnAlZumar.API.Services.Customers;

public interface ICustomerService
{
    Task<PagedResultDto<CustomerResponseDto>> GetAllAsync(CustomerFilterDto filter);
    Task<CustomerResponseDto> GetByIdAsync(int id);
    Task<CustomerResponseDto> CreateAsync(CreateCustomerDto dto);
    Task<CustomerResponseDto> UpdateAsync(int id, UpdateSalesCustomerDto dto);
    Task DeleteAsync(int id);
    Task<CustomerResponseDto> AdjustDebtAsync(int id, AdjustCustomerDebtDto dto);
}