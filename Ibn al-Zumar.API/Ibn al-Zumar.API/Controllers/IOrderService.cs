using IbnAlZumar.API.DTOs.Sales;

namespace IbnAlZumar.API.Services.Sales
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateAsync(CreateOrderDto dto);
    }
}