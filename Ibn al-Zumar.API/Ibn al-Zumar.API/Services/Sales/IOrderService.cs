using IbnAlZumar.API.DTOs.Sales;
using IbnAlZumar.Domain.Enums;

namespace Services.Sales
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateAsync(CreateOrderDto dto);
        Task<List<CustomerOrderDto>> GetMyOrdersAsync(string userEmail);
        Task<CustomerOrderDto> GetOrderDetailsAsync(int id, string? userEmail, bool isAdminOrMod);
        Task<IEnumerable<object>> GetAllOrdersAsync();
        Task AdvanceOrderStatusAsync(int id);

        /// <summary>
        /// تحديث حالة الطلب مباشرة إلى حالة محددة
        /// </summary>
        Task UpdateOrderStatusAsync(int id, OrderStatus status);

        // ================= الدوال الجديدة لإلغاء الطلب =================
        Task RequestCancelOrderAsync(int orderId, string reason, string userEmail);
        Task ApproveCancelOrderAsync(int orderId);
    }
}