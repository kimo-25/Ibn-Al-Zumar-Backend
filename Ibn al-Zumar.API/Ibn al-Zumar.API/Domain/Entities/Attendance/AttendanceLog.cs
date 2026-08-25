using IbnAlZumar.Domain.Common;
using IbnAlZumar.Domain.Entities.Identity;

namespace IbnAlZumar.Domain.Entities.Attendance;

public enum AttendanceStatus
{
    CheckedIn = 1,
    CheckedOut = 2,
    LeftEarlyWithIssue = 3
}

public class AttendanceLog : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }

    public AttendanceStatus Status { get; set; } = AttendanceStatus.CheckedIn;

    public string? Notes { get; set; }

    public double? WorkedHours { get; set; }
}
