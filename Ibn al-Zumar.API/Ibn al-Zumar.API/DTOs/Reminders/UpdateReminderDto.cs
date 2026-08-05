using System.ComponentModel.DataAnnotations;
using IbnAlZumar.Domain.Enums;

namespace IbnAlZumar.API.DTOs.Reminders
{
    public class UpdateReminderDto : CreateReminderDto
    {
        public bool IsActive { get; set; } = true;
    }
}