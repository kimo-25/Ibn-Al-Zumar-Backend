using System;
using System.ComponentModel.DataAnnotations;
using IbnAlZumar.Domain.Enums;

namespace IbnAlZumar.Domain.Entities.Reminders
{
    public class Reminder
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Text { get; set; } = string.Empty;

        [Required]
        public ReminderType Type { get; set; }

        [MaxLength(200)]
        public string? Source { get; set; }

        [MaxLength(100)]
        public string? SurahName { get; set; }

        public int? AyahNumber { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}