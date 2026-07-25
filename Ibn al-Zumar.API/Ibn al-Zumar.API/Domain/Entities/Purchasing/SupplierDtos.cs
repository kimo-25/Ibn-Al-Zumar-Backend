using System.ComponentModel.DataAnnotations;

namespace IbnAlZumar.API.DTOs.Purchasing
{
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

    public class UpdateSupplierDto : CreateSupplierDto
    {
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
    }
}
