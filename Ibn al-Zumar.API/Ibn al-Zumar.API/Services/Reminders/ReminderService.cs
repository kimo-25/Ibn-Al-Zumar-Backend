using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IbnAlZumar.API.DTOs.Reminders;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.Domain.Entities.Reminders;
using IbnAlZumar.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace IbnAlZumar.API.Services.Reminders
{
    public class ReminderService : IReminderService
    {
        private readonly ApplicationDbContext _context;
        private static readonly Random _random = new Random();

        public ReminderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ReminderDto?> GetRandomReminderAsync()
        {
            var count = await _context.Reminders.CountAsync(r => r.IsActive);
            if (count == 0) return null;

            var randomIndex = _random.Next(0, count);

            var reminder = await _context.Reminders
                .Where(r => r.IsActive)
                .Skip(randomIndex)
                .FirstOrDefaultAsync();

            return reminder == null ? null : MapToDto(reminder);
        }

        public async Task<IEnumerable<ReminderDto>> GetAllRemindersAsync(bool includeInactive = false)
        {
            var query = _context.Reminders.AsNoTracking();
            if (!includeInactive)
            {
                query = query.Where(r => r.IsActive);
            }

            var list = await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
            return list.Select(MapToDto);
        }

        public async Task<ReminderDto?> GetByIdAsync(int id)
        {
            var reminder = await _context.Reminders.FindAsync(id);
            return reminder == null ? null : MapToDto(reminder);
        }

        public async Task<ReminderDto> CreateReminderAsync(CreateReminderDto dto)
        {
            var reminder = new Reminder
            {
                Text = dto.Text.Trim(),
                Type = dto.Type,
                Source = dto.Source?.Trim(),
                SurahName = dto.SurahName?.Trim(),
                AyahNumber = dto.AyahNumber,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reminders.Add(reminder);
            await _context.SaveChangesAsync();

            return MapToDto(reminder);
        }

        public async Task<bool> UpdateReminderAsync(int id, UpdateReminderDto dto)
        {
            var reminder = await _context.Reminders.FindAsync(id);
            if (reminder == null) return false;

            reminder.Text = dto.Text.Trim();
            reminder.Type = dto.Type;
            reminder.Source = dto.Source?.Trim();
            reminder.SurahName = dto.SurahName?.Trim();
            reminder.AyahNumber = dto.AyahNumber;
            reminder.IsActive = dto.IsActive;
            reminder.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleStatusAsync(int id)
        {
            var reminder = await _context.Reminders.FindAsync(id);
            if (reminder == null) return false;

            reminder.IsActive = !reminder.IsActive;
            reminder.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var reminder = await _context.Reminders.FindAsync(id);
            if (reminder == null) return false;

            reminder.IsActive = false;
            reminder.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        private static ReminderDto MapToDto(Reminder reminder) => new ReminderDto
        {
            Id = reminder.Id,
            Text = reminder.Text,
            Type = reminder.Type,
            Source = reminder.Source,
            SurahName = reminder.SurahName,
            AyahNumber = reminder.AyahNumber,
            IsActive = reminder.IsActive
        };
    }
}