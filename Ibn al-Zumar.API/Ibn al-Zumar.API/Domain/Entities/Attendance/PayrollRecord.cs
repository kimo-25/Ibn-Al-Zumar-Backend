using IbnAlZumar.Domain.Common;
using IbnAlZumar.Domain.Entities.Identity;

namespace IbnAlZumar.Domain.Entities.Attendance;

public class PayrollRecord : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }

    public double TotalHours { get; set; }
    public decimal TotalSalary { get; set; }

    public bool IsPaid { get; set; }
    public DateTime? PaymentDate { get; set; }
}
