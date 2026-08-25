using System.Text.Json;
using IbnAlZumar.API.DTOs.Attendance;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.Domain.Entities.Attendance;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace IbnAlZumar.API.Services.Attendance;

public class AttendanceService : IAttendanceService
{
    private const double MatchThreshold = 0.75;

    private readonly ApplicationDbContext _db;
    private readonly IVoiceVerificationService _voiceService;

    public AttendanceService(ApplicationDbContext db, IVoiceVerificationService voiceService)
    {
        _db = db;
        _voiceService = voiceService;
    }

    public async Task<VoiceEnrollResultDto> EnrollVoiceAsync(int userId, IFormFile audioFile, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null)
        {
            return new VoiceEnrollResultDto
            {
                UserId = userId,
                Success = false,
                Message = "المستخدم غير موجود."
            };
        }

        await using var stream = audioFile.OpenReadStream();
        var embedding = await _voiceService.ExtractVoiceEmbeddingAsync(stream, audioFile.FileName, cancellationToken);

        if (embedding.Length == 0)
        {
            return new VoiceEnrollResultDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Success = false,
                Message = "تعذر استخراج بصمة صوتية صالحة من التسجيل، يرجى إعادة المحاولة في مكان أهدأ."
            };
        }

        user.VoiceEmbedding = JsonSerializer.Serialize(embedding);
        await _db.SaveChangesAsync(cancellationToken);

        return new VoiceEnrollResultDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Success = true,
            Message = "تم تسجيل البصمة الصوتية بنجاح."
        };
    }

    public async Task<AttendanceCheckResultDto> ProcessVoiceAttendanceAsync(IFormFile audioFile, string? notes, CancellationToken cancellationToken = default)
    {
        await using var stream = audioFile.OpenReadStream();
        var incomingEmbedding = await _voiceService.ExtractVoiceEmbeddingAsync(stream, audioFile.FileName, cancellationToken);

        var enrolledUsers = await _db.Users
            .Where(u => u.VoiceEmbedding != null && u.IsActive)
            .ToListAsync(cancellationToken);

        int? bestUserId = null;
        double bestScore = 0;

        foreach (var candidate in enrolledUsers)
        {
            float[]? candidateEmbedding;
            try
            {
                candidateEmbedding = JsonSerializer.Deserialize<float[]>(candidate.VoiceEmbedding!);
            }
            catch (JsonException)
            {
                continue;
            }

            if (candidateEmbedding == null || candidateEmbedding.Length == 0)
            {
                continue;
            }

            var score = _voiceService.CalculateCosineSimilarity(incomingEmbedding, candidateEmbedding);
            if (score > bestScore)
            {
                bestScore = score;
                bestUserId = candidate.Id;
            }
        }

        if (bestUserId == null || bestScore < MatchThreshold)
        {
            return new AttendanceCheckResultDto
            {
                Success = false,
                Message = "تعذر التعرف على الصوت. حاول مرة أخرى بصوت أوضح أو تواصل مع الإدارة.",
                MatchConfidence = bestScore
            };
        }

        var matchedUser = enrolledUsers.First(u => u.Id == bestUserId);
        var todayUtc = DateTime.UtcNow.Date;

        var openLog = await _db.AttendanceLogs
            .Where(a => a.UserId == matchedUser.Id
                        && a.CheckOutTime == null
                        && a.CheckInTime >= todayUtc
                        && a.CheckInTime < todayUtc.AddDays(1))
            .OrderByDescending(a => a.CheckInTime)
            .FirstOrDefaultAsync(cancellationToken);

        if (openLog == null)
        {
            var newLog = new AttendanceLog
            {
                UserId = matchedUser.Id,
                CheckInTime = DateTime.UtcNow,
                Status = AttendanceStatus.CheckedIn
            };

            _db.AttendanceLogs.Add(newLog);
            await _db.SaveChangesAsync(cancellationToken);

            return new AttendanceCheckResultDto
            {
                Success = true,
                Message = $"تم تسجيل حضور {matchedUser.FullName} بنجاح.",
                UserId = matchedUser.Id,
                FullName = matchedUser.FullName,
                Action = "CheckIn",
                Timestamp = newLog.CheckInTime,
                MatchConfidence = bestScore
            };
        }

        var checkOutTime = DateTime.UtcNow;
        var workedHours = Math.Round((checkOutTime - openLog.CheckInTime).TotalHours, 2);

        openLog.CheckOutTime = checkOutTime;
        openLog.WorkedHours = workedHours;
        openLog.Notes = notes;
        openLog.Status = !string.IsNullOrWhiteSpace(notes)
            ? AttendanceStatus.LeftEarlyWithIssue
            : AttendanceStatus.CheckedOut;

        await _db.SaveChangesAsync(cancellationToken);

        return new AttendanceCheckResultDto
        {
            Success = true,
            Message = $"تم تسجيل انصراف {matchedUser.FullName} بعد {workedHours:0.##} ساعة عمل.",
            UserId = matchedUser.Id,
            FullName = matchedUser.FullName,
            Action = "CheckOut",
            Timestamp = checkOutTime,
            WorkedHours = workedHours,
            MatchConfidence = bestScore
        };
    }

    public async Task<List<AttendanceLogDto>> GetLogsAsync(DateTime? from, DateTime? to, CancellationToken cancellationToken = default)
    {
        var query = _db.AttendanceLogs.Include(a => a.User).AsQueryable();

        if (from.HasValue)
        {
            query = query.Where(a => a.CheckInTime >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(a => a.CheckInTime <= to.Value);
        }

        return await query
            .OrderByDescending(a => a.CheckInTime)
            .Select(a => new AttendanceLogDto
            {
                Id = a.Id,
                UserId = a.UserId,
                FullName = a.User.FullName,
                CheckInTime = a.CheckInTime,
                CheckOutTime = a.CheckOutTime,
                Status = a.Status.ToString(),
                Notes = a.Notes,
                WorkedHours = a.WorkedHours
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<PayrollSummaryDto>> GetPayrollSummaryAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var endExclusive = endDate.Date.AddDays(1);

        var logs = await _db.AttendanceLogs
            .Include(a => a.User)
            .Where(a => a.CheckInTime >= startDate.Date
                        && a.CheckInTime < endExclusive
                        && a.WorkedHours != null)
            .ToListAsync(cancellationToken);

        return logs
            .GroupBy(a => a.User)
            .Select(group => new PayrollSummaryDto
            {
                UserId = group.Key.Id,
                FullName = group.Key.FullName,
                HourlyRate = group.Key.HourlyRate,
                TotalHours = Math.Round(group.Sum(a => a.WorkedHours ?? 0), 2),
                TotalSalary = Math.Round((decimal)group.Sum(a => a.WorkedHours ?? 0) * group.Key.HourlyRate, 2)
            })
            .OrderBy(p => p.FullName)
            .ToList();
    }
}
