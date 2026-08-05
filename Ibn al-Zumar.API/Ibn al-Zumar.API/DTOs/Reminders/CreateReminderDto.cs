using System.ComponentModel.DataAnnotations;
using IbnAlZumar.Domain.Enums;

namespace IbnAlZumar.API.DTOs.Reminders
{
    public class CreateReminderDto
    {
        [Required(ErrorMessage = "نص التذكير مطلوب")]
        [StringLength(1000, ErrorMessage = "النص لا يمكن أن يتجاوز 1000 حرف")]
        public string Text { get; set; } = string.Empty;

        [Required(ErrorMessage = "نوع التذكير مطلوب")]
        public ReminderType Type { get; set; }

        [StringLength(200)]
        public string? Source { get; set; }

        [StringLength(100)]
        public string? SurahName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "رقم الآية يجب أن يكون أكبر من 0")]
        public int? AyahNumber { get; set; }
    }
}