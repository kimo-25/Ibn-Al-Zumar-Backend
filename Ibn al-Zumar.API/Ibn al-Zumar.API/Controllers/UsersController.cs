using IbnAlZumar.API.DTOs.Identity;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.API.Services.Identity;
using Microsoft.EntityFrameworkCore;
using IbnAlZumar.Persistence.Seed; // إضافة الـ namespace الخاص بـ DataSeeder
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IbnAlZumar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserManagementService _userService;
    private readonly ApplicationDbContext _db;

    public UsersController(IUserManagementService userService, ApplicationDbContext db)
    {
        _userService = userService;
        _db = db;
    }

    [HttpGet]
    [Authorize(Policy = DataSeeder.PermissionCodes.UsersManage)]
    public async Task<IActionResult> GetUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _userService.GetUsersAsync(pageNumber, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = DataSeeder.PermissionCodes.UsersManage)]
    public async Task<IActionResult> GetUserById(int id)
    {
        var result = await _userService.GetUserByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = DataSeeder.PermissionCodes.UsersManage)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        var result = await _userService.CreateUserAsync(dto);
        return CreatedAtAction(nameof(GetUserById), new { id = result.Id }, result);
    }

    [HttpGet("{userId}/profile-summary")]
    [Authorize(Roles = "Admin,SuperAdmin,STORE_OWNER")]
    public async Task<IActionResult> GetProfileSummary(int userId, [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken cancellationToken)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null) return NotFound(new { message = "الموظف غير موجود." });

        var query = _db.AttendanceLogs.AsNoTracking().Where(a => a.UserId == userId);
        if (from.HasValue) query = query.Where(a => a.CheckInTime >= from.Value);
        if (to.HasValue) query = query.Where(a => a.CheckInTime < to.Value.Date.AddDays(1));
        var logs = await query.OrderByDescending(a => a.CheckInTime).ToListAsync(cancellationToken);
        var totalMinutes = logs.Sum(a => a.WorkedMinutes ?? (a.WorkedHours.HasValue ? (int)Math.Round(a.WorkedHours.Value * 60, MidpointRounding.AwayFromZero) : 0));

        return Ok(new EmployeeProfileSummaryDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Username = user.Username,
            Email = user.Email,
            IsActive = user.IsActive,
            HourlyRate = user.HourlyRate,
            IsVoiceEnrolled = !string.IsNullOrWhiteSpace(user.VoiceEmbedding),
            VoiceEnrolledAtUtc = user.VoiceEnrolledAtUtc,
            TotalWorkedMinutes = totalMinutes,
            TotalWorkedHours = Math.Round(totalMinutes / 60d, 2),
            CalculatedSalary = Math.Round(totalMinutes / 60m * user.HourlyRate, 2),
            AttendanceCount = logs.Count,
            Attendance = logs.Select(a => new EmployeeAttendanceRecordDto
            {
                Id = a.Id,
                CheckInTime = a.CheckInTime,
                CheckOutTime = a.CheckOutTime,
                WorkedMinutes = a.WorkedMinutes,
                WorkedHours = a.WorkedHours,
                Status = a.Status.ToString(),
                VerificationMethod = a.VerificationMethod.ToString(),
                Notes = a.Notes
            }).ToList()
        });
    }

    [HttpPatch("{userId}/hourly-rate")]
    [Authorize(Roles = "Admin,SuperAdmin,STORE_OWNER")]
    public async Task<IActionResult> UpdateHourlyRate(int userId, [FromBody] UpdateHourlyRateDto dto, CancellationToken cancellationToken)
    {
        if (dto == null || dto.HourlyRate < 0) return BadRequest(new { message = "الأجر بالساعة يجب أن يكون صفراً أو قيمة موجبة." });
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null) return NotFound(new { message = "الموظف غير موجود." });
        user.HourlyRate = decimal.Round(dto.HourlyRate, 2);
        await _db.SaveChangesAsync(cancellationToken);
        return Ok(new { userId = user.Id, hourlyRate = user.HourlyRate });
    }

    [HttpPut("{id}/roles")]
    [Authorize(Policy = DataSeeder.PermissionCodes.UsersManage)]
    public async Task<IActionResult> UpdateUserRoles(int id, [FromBody] UpdateUserRolesDto dto)
    {
        await _userService.UpdateUserRolesAsync(id, dto);
        return NoContent();
    }

    [HttpPatch("{id}/toggle-status")]
    [Authorize(Policy = DataSeeder.PermissionCodes.UsersManage)]
    public async Task<IActionResult> ToggleUserStatus(int id)
    {
        await _userService.ToggleUserStatusAsync(id);
        return NoContent();
    }
}