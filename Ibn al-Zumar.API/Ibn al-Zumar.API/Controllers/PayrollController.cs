using IbnAlZumar.API.Services.Attendance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IbnAlZumar.API.Controllers;

[ApiController]
[Route("api/payroll")]
[Authorize(Roles = "Admin,SuperAdmin,STORE_OWNER")]
public class PayrollController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;

    public PayrollController(IAttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    /// <summary>
    /// ملخص الرواتب: إجمالي الساعات × أجر الساعة لكل موظف خلال الفترة المحددة.
    /// </summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        if (endDate.Date < startDate.Date)
        {
            return BadRequest(new { message = "تاريخ النهاية يجب أن يكون بعد تاريخ البداية." });
        }

        var result = await _attendanceService.GetPayrollSummaryAsync(startDate, endDate);
        return Ok(result);
    }
}
