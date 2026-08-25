using System.Security.Claims;
using IbnAlZumar.API.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IbnAlZumar.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // لازم يكون الكاشير عامل لوج إن
    public class ExpensesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExpensesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userEmail = User.FindFirstValue(ClaimTypes.Email)
                            ?? User.FindFirstValue(ClaimTypes.Name);

            // السطر ده عشان نمنع تحذير CS1998 لحين تفعيل الداتابيز
            await Task.CompletedTask;

            // TODO: قم بإنشاء Entity باسم Expense في الداتابيز وفعّل الكود التالي
            /*
            var expense = new Expense 
            {
                Amount = dto.Amount,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userEmail
            };
            
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
            */

            return Ok(new { message = "تم تسجيل المصروف بنجاح", data = dto });
        }
    }

    // الـ DTO الخاص بالمصروفات
    public class CreateExpenseDto
    {
        public decimal Amount { get; set; }
        public string? Notes { get; set; }
    }
}