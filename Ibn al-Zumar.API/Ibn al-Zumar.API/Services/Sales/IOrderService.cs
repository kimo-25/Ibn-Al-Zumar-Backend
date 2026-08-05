using IbnAlZumar.API.DTOs.Sales;

namespace Services.Sales
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateAsync(CreateOrderDto dto);
        Task<List<CustomerOrderDto>> GetMyOrdersAsync(string userEmail);

        Task<IEnumerable<object>> GetAllOrdersAsync();
        Task AdvanceOrderStatusAsync(int id);
    }
}