namespace DTOs.Sales
{
    public class UpdateSalesCustomerDto
    {
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public decimal? CreditLimit { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
