using IbnAlZumar.Domain.Enums;

namespace IbnAlZumar.API.DTOs.Reminders
{
    public class ReminderDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public ReminderType Type { get; set; }
        public string? Source { get; set; }
        public string? SurahName { get; set; }
        public int? AyahNumber { get; set; }
        public bool IsActive { get; set; }
    }
}