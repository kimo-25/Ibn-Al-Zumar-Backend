using System.ComponentModel.DataAnnotations;

namespace IbnAlZumar.API.DTOs.Purchasing
{
    // ============================================================
    // Supplier Core DTOs (الخاصة بإنشاء وتعديل وعرض الموردين)
    // ============================================================

    public class CreateSupplierDto
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? ContactPerson { get; set; }

        [MaxLength(30)]
        public string? Phone { get; set; }

        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(300)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? TaxId { get; set; }
    }

    public class UpdateSupplierDto
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? ContactPerson { get; set; }

        [MaxLength(30)]
        public string? Phone { get; set; }

        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(300)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? TaxId { get; set; }

        public decimal CurrentBalance { get; set; }
    }

    public class SupplierResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? TaxId { get; set; }
        public decimal CurrentBalance { get; set; }
        public int TotalPurchaseOrders { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // ============================================================
    // Supplier Accounting DTOs (الخاصة بالدفوعات وكشف الحساب)
    // ============================================================

    public class CreateSupplierPaymentDto
    {
        [Required]
        public int SupplierId { get; set; }

        /// <summary>Optional — link this payment to a specific Purchase Order, or leave null for a general balance payment.</summary>
        public int? PurchaseOrderId { get; set; }

        [Required, Range(0.01, double.MaxValue, ErrorMessage = "قيمة الدفعة يجب أن تكون أكبر من صفر")]
        public decimal Amount { get; set; }

        /// <summary>One of: Cash, BankTransfer, Cheque.</summary>
        [Required]
        public string PaymentMethod { get; set; } = string.Empty;

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        public string? Notes { get; set; }

        /// <summary>Optional — user who recorded the payment.</summary>
        public int? CreatedByUserId { get; set; }
    }

    public class SupplierPaymentResponseDto
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public int? PurchaseOrderId { get; set; }
        public string? PurchaseOrderNumber { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public string? Notes { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class SupplierLedgerEntryDto
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal RunningBalance { get; set; }
        public int? RelatedPurchaseOrderId { get; set; }
        public string? RelatedPurchaseOrderNumber { get; set; }
        public int? RelatedPaymentId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Full picture of a supplier: profile info + complete statement of account (ledger)
    /// + payment history. Powers the "Supplier Details / Statement of Account" screen.
    /// </summary>
    public class SupplierDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? TaxId { get; set; }
        public decimal CurrentBalance { get; set; }
        public int TotalPurchaseOrders { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<SupplierLedgerEntryDto> LedgerEntries { get; set; } = new();
        public List<SupplierPaymentResponseDto> Payments { get; set; } = new();
    }
}