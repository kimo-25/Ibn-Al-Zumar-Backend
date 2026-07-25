namespace IbnAlZumar.API.DTOs.Customers;

public class CustomerResponseDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? Governorate { get; set; }
    public bool IsRegistered { get; set; }
    public decimal CreditLimit { get; set; }
    public decimal CurrentBalance { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCustomerDto
{
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? Governorate { get; set; }
    public decimal CreditLimit { get; set; } = 0;
    public bool IsRegistered { get; set; } = false;
}

public class UpdateSalesCustomerDto
{
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? Governorate { get; set; }
    public decimal CreditLimit { get; set; }
}

public class CustomerFilterDto
{
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class AdjustCustomerDebtDto
{
    public decimal Amount { get; set; }
    public string? Reason { get; set; }
}