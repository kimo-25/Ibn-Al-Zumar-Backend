using System.ComponentModel.DataAnnotations;

namespace IbnAlZumar.API.DTOs.Sales
{
    public class CreateCustomerDto
    {
        [Required(ErrorMessage = "اسم العميل مطلوب")]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(300)]
        public string? Address { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "حد الائتمان يجب أن يكون أكبر من أو يساوي صفر")]
        public decimal? CreditLimit { get; set; }

        public bool IsActive { get; set; } = true;

        // Note: TotalDebt is intentionally NOT settable here — it always starts at 0.
        // Use POST /api/customers/{id}/adjust-debt for an opening balance so it's captured
        // in the CustomerLedgerEntries audit trail like every other debt change.
    }

    public class UpdateCustomerDto
    {
        [Required(ErrorMessage = "اسم العميل مطلوب")]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(300)]
        public string? Address { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "حد الائتمان يجب أن يكون أكبر من أو يساوي صفر")]
        public decimal? CreditLimit { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class CustomerResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public decimal TotalDebt { get; set; }
        public decimal? CreditLimit { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>Bound from query string on GET /api/customers.</summary>
    public class CustomerFilterDto
    {
        /// <summary>Matches against Name, Phone and Email.</summary>
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }

        private int _pageNumber = 1;
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }

        private int _pageSize = 20;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value switch
            {
                < 1 => 20,
                > 100 => 100,
                _ => value
            };
        }
    }

    /// <summary>Records a signed change to a customer's debt (new credit sale, payment received, correction...).</summary>
    public class AdjustCustomerDebtDto
    {
        /// <summary>Positive increases debt (e.g. credit sale); negative decreases it (e.g. payment received). Must not be zero.</summary>
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "سبب التسوية مطلوب")]
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;
    }
}