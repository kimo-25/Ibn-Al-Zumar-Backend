using System.Collections.Generic;
using System.Threading.Tasks;
using IbnAlZumar.API.DTOs.Reminders;

namespace IbnAlZumar.API.Services.Reminders
{
    public interface IReminderService
    {
        Task<ReminderDto?> GetRandomReminderAsync();
        Task<IEnumerable<ReminderDto>> GetAllRemindersAsync(bool includeInactive = false);
        Task<ReminderDto?> GetByIdAsync(int id);
        Task<ReminderDto> CreateReminderAsync(CreateReminderDto dto);
        Task<bool> UpdateReminderAsync(int id, UpdateReminderDto dto);
        Task<bool> ToggleStatusAsync(int id);
        Task<bool> SoftDeleteAsync(int id);
    }
}