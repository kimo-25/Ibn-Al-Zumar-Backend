using IbnAlZumar.API.DTOs.Purchasing;

namespace IbnAlZumar.API.Services.Purchasing
{
    public interface IPurchasingService
    {
        Task<SupplierResponseDto> CreateSupplierAsync(CreateSupplierDto dto);
        Task<PurchaseOrderResponseDto> CreatePurchaseOrderAsync(CreatePurchaseOrderDto dto);
        Task<PurchaseOrderResponseDto> ApprovePurchaseOrderAsync(ApprovePurchaseOrderDto dto);
    }
}
