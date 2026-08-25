using IbnAlZumar.API.DTOs.Attendance;
using Microsoft.AspNetCore.Http;

namespace IbnAlZumar.API.Services.Attendance;

public interface IAttendanceService
{
    Task<VoiceEnrollResultDto> EnrollVoiceAsync(int userId, IFormFile audioFile, CancellationToken cancellationToken = default);

    Task<AttendanceCheckResultDto> ProcessVoiceAttendanceAsync(IFormFile audioFile, string? notes, CancellationToken cancellationToken = default);

    Task<List<AttendanceLogDto>> GetLogsAsync(DateTime? from, DateTime? to, CancellationToken cancellationToken = default);

    Task<List<PayrollSummaryDto>> GetPayrollSummaryAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}