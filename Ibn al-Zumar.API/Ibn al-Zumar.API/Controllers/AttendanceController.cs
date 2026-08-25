using System.Security.Claims;
using IbnAlZumar.API.Services.Attendance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IbnAlZumar.API.Controllers;

[ApiController]
[Route("api/attendance")]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;

    public AttendanceController(IAttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    /// <summary>
    /// تسجيل بصمة صوت الموظف الحالي لأول مرة (Enrollment).
    /// </summary>
    [HttpPost("enroll-voice")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> EnrollVoice(IFormFile audio)
    {
        if (audio == null || audio.Length == 0)
        {
            return BadRequest(new { message = "الرجاء إرفاق تسجيل صوتي." });
        }

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "تعذر تحديد هوية المستخدم الحالي." });
        }

        var result = await _attendanceService.EnrollVoiceAsync(userId, audio);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// تسجيل حضور/انصراف بالصوت من شاشة الكاشير
    /// </summary>
    [HttpPost("voice-check")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> VoiceCheck(IFormFile audio, [FromForm] string? notes)
    {
        if (audio == null || audio.Length == 0)
        {
            return BadRequest(new { message = "الرجاء إرفاق تسجيل صوتي." });
        }

        var result = await _attendanceService.ProcessVoiceAttendanceAsync(audio, notes);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// سجل الحضور والانصراف الكامل — للأدمن فقط.
    /// </summary>
    [HttpGet("logs")]
    [Authorize(Roles = "Admin,SuperAdmin,STORE_OWNER")]
    public async Task<IActionResult> GetLogs([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var result = await _attendanceService.GetLogsAsync(from, to);
        return Ok(result);
    }
}