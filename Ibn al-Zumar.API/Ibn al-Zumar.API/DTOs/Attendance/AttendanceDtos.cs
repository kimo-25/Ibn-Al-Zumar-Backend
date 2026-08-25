namespace IbnAlZumar.API.DTOs.Attendance;

public class VoiceEnrollResultDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class AttendanceCheckResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? UserId { get; set; }
    public string? FullName { get; set; }

    /// <summary>
    /// "CheckIn" أو "CheckOut" حسب الحالة التي تمت معالجتها.
    /// </summary>
    public string? Action { get; set; }

    public DateTime? Timestamp { get; set; }
    public double? WorkedHours { get; set; }
    public double? MatchConfidence { get; set; }
}

public class AttendanceLogDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateTime CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public double? WorkedHours { get; set; }
}

public class PayrollSummaryDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public double TotalHours { get; set; }
    public decimal TotalSalary { get; set; }
}
