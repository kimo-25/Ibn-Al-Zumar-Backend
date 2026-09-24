using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IbnAlZumar.API.Controllers
{
    /// <summary>
    /// H-09: there is currently no Expense entity/DbSet (see ARCHITECTURE.md §3.7 — 
    /// "controller stub, no entity"). Returning a fake 200 OK here would tell the POS
    /// cashier a cash expense was recorded when it was silently discarded — a direct
    /// cash-accounting risk. Until Expense is a real, persisted entity, this endpoint
    /// honestly reports itself as not implemented instead.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExpensesController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status501NotImplemented)]
        public IActionResult Create([FromBody] CreateExpenseRequestDto dto)
        {
            return StatusCode(StatusCodes.Status501NotImplemented, new
            {
                message = "ميزة تسجيل المصاريف غير مفعّلة بعد على السيرفر. لم يتم حفظ أي بيانات.",
                messageEn = "Expense persistence is not implemented yet. Nothing was saved."
            });
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status501NotImplemented)]
        public IActionResult GetAll()
        {
            return StatusCode(StatusCodes.Status501NotImplemented, new
            {
                message = "ميزة المصاريف غير مفعّلة بعد على السيرفر.",
                messageEn = "Expense persistence is not implemented yet."
            });
        }
    }

    // Placeholder request shape matching what PosCheckoutPage.jsx currently sends
    // ({ amount, notes }). Move this into DTOs/Finance/ once Expense is a real
    // entity and replace with a proper CreateExpenseDto + validation.
    public sealed class CreateExpenseRequestDto
    {
        public decimal Amount { get; set; }
        public string? Notes { get; set; }
    }
}