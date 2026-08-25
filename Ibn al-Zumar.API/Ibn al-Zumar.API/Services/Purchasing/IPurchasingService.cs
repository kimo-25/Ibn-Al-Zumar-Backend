using IbnAlZumar.API.DTOs.Purchasing;

namespace IbnAlZumar.API.Services.Purchasing
{
    public interface IPurchasingService
    {
        Task<List<SupplierResponseDto>> GetSuppliersAsync();
        Task<SupplierResponseDto> GetSupplierByIdAsync(int id);
        Task<SupplierResponseDto> CreateSupplierAsync(CreateSupplierDto dto);
        Task<SupplierResponseDto> UpdateSupplierAsync(int id, UpdateSupplierDto dto);
        Task DeleteSupplierAsync(int id);

        Task<List<PurchaseOrderResponseDto>> GetPurchaseOrdersAsync();
        Task<PurchaseOrderResponseDto> GetPurchaseOrderByIdAsync(int id);
        Task<PurchaseOrderResponseDto> CreatePurchaseOrderAsync(CreatePurchaseOrderDto dto);

        /// <summary>
        /// Receives a Draft purchase order: bumps ProductStock.QuantityOnHand (creating the
        /// ProductStock row if it doesn't exist yet), writes one InventoryTransaction per line,
        /// sets ProductStock.LastRestockedAt, updates Product.CurrentCostPrice to the latest
        /// purchase cost, increases Supplier.CurrentBalance by the order total, and writes a
        /// SupplierLedgerEntry (PurchaseInvoice) capturing the resulting running balance.
        /// </summary>
        Task<PurchaseOrderResponseDto> ReceivePurchaseOrderAsync(ApprovePurchaseOrderDto dto);

        // ============================================================
        // Supplier Accounting (Ledger & Payments)
        // ============================================================

        /// <summary>
        /// Records a payment made to a supplier: saves the SupplierPayment, decreases
        /// Supplier.CurrentBalance by the amount, and writes a SupplierLedgerEntry (Payment)
        /// capturing the resulting running balance.
        /// </summary>
        Task<SupplierPaymentResponseDto> CreateSupplierPaymentAsync(CreateSupplierPaymentDto dto);

        /// <summary>Returns the full statement of account (running balance) for a supplier, oldest first.</summary>
        Task<List<SupplierLedgerEntryDto>> GetSupplierLedgerAsync(int supplierId);

        /// <summary>Returns supplier profile info combined with its full ledger and payment history.</summary>
        Task<SupplierDetailsDto> GetSupplierDetailsAsync(int supplierId);
    }
}