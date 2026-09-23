namespace IbnAlZumar.API.DTOs.Identity;

public sealed class UpdateHourlyRateDto
{
    public decimal HourlyRate { get; set; }
}

public sealed class EmployeeProfileSummaryDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsActive { get; set; }
    public decimal HourlyRate { get; set; }
    public bool IsVoiceEnrolled { get; set; }
    public DateTime? VoiceEnrolledAtUtc { get; set; }
    public int TotalWorkedMinutes { get; set; }
    public double TotalWorkedHours { get; set; }
    public decimal CalculatedSalary { get; set; }
    public int AttendanceCount { get; set; }
    public List<EmployeeAttendanceRecordDto> Attendance { get; set; } = new();
}

public sealed class EmployeeAttendanceRecordDto
{
    public int Id { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public int? WorkedMinutes { get; set; }
    public double? WorkedHours { get; set; }
    public string Status { get; set; } = string.Empty;
    public string VerificationMethod { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
