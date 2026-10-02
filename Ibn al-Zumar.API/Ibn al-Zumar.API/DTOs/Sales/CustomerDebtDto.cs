namespace IbnAlZumar.API.DTOs.Sales;

public class CustomerDebtDto
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }
    public string? CustomerEmail { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal CreditLimit { get; set; }
    public DateTime? LastPaymentDate { get; set; }
    public bool IsScheduleActive { get; set; }
    public DateTime? LastReminderSentAt { get; set; }
    public DateTime? NextReminderDueAt { get; set; }
    public int ReminderCount { get; set; }
}
