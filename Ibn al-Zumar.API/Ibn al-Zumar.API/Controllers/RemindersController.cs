using System.Threading.Tasks;
using IbnAlZumar.API.DTOs.Reminders;
using IbnAlZumar.API.Services.Reminders;
using IbnAlZumar.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IbnAlZumar.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RemindersController : ControllerBase
    {
        private readonly IReminderService _reminderService;

        public RemindersController(IReminderService reminderService)
        {
            _reminderService = reminderService;
        }

        [HttpGet("random")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRandom()
        {
            var reminder = await _reminderService.GetRandomReminderAsync();
            if (reminder == null) return NotFound(new { message = "لا توجد أذكار نشطة حالياً" });
            return Ok(reminder);
        }

        [HttpGet("admin/all")]
        [Authorize(Roles = "Owner, Moderator")]
        public async Task<IActionResult> GetAll()
        {
            var reminders = await _reminderService.GetAllRemindersAsync(includeInactive: true);
            return Ok(reminders);
        }

        [HttpPost("admin")]
        [Authorize(Roles = "Owner, Moderator")]
        public async Task<IActionResult> Create([FromBody] CreateReminderDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _reminderService.CreateReminderAsync(dto);
            return CreatedAtAction(nameof(GetRandom), new { id = created.Id }, created);
        }

        [HttpPut("admin/{id:int}")]
        [Authorize(Roles = "Owner, Moderator")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateReminderDto dto)
        {
            var result = await _reminderService.UpdateReminderAsync(id, dto);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPatch("admin/{id:int}/toggle-status")]
        [Authorize(Roles = "Owner, Moderator")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = await _reminderService.ToggleStatusAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("admin/{id:int}")]
        [Authorize(Roles = "Owner, Moderator")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _reminderService.SoftDeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}