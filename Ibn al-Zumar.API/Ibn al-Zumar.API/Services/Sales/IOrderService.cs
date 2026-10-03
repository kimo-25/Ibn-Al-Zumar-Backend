using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IbnAlZumar.API.DTOs.Common;
using IbnAlZumar.API.DTOs.Sales;
using IbnAlZumar.Domain.Enums;

namespace IbnAlZumar.Api.Services.Sales;

public interface IOrderService
{
    Task<OrderResponseDto> CreateAsync(CreateOrderDto dto);

    Task<List<CustomerOrderDto>> GetMyOrdersAsync(string userEmail);

    Task<CustomerOrderDto> GetOrderDetailsAsync(
        int id,
        string? userEmail,
        bool isAdminOrMod);

    Task<PagedResultDto<OrderListDto>> GetAllOrdersAsync(
        OrderFilterDto filter,
        CancellationToken ct = default);

    Task AdvanceOrderStatusAsync(int id);

    /// <summary>
    /// تحديث حالة الطلب مباشرة إلى حالة محددة.
    /// </summary>
    Task UpdateOrderStatusAsync(int id, OrderStatus status);

    /// <summary>
    /// طلب إلغاء الطلب من العميل.
    /// </summary>
    Task RequestCancelOrderAsync(
        int orderId,
        string reason,
        string userEmail);

    /// <summary>
    /// الموافقة على إلغاء الطلب من الإدارة.
    /// </summary>
    Task ApproveCancelOrderAsync(int orderId);

    /// <summary>
    /// تعديل أصناف الطلب والكميات وإعادة حساب الإجمالي.
    /// </summary>
    Task<OrderResponseDto> UpdateOrderAsync(
        int id,
        UpdateOrderDto dto);

    /// <summary>
    /// تسجيل أو تصحيح المبالغ المدفوعة وتحديث مديونية العميل.
    /// </summary>
    Task ApplyPaymentToOrderAsync(
        int id,
        UpdateOrderPaymentDto dto);
}