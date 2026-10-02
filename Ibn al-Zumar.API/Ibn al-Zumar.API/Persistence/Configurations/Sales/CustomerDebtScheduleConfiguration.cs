using IbnAlZumar.Domain.Entities.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IbnAlZumar.API.Persistence.Configurations.Sales;

public class CustomerDebtScheduleConfiguration : IEntityTypeConfiguration<CustomerDebtSchedule>
{
    public void Configure(EntityTypeBuilder<CustomerDebtSchedule> builder)
    {
        builder.ToTable("CustomerDebtSchedules");

        builder.HasOne(s => s.Customer)
            .WithMany()
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        // One schedule row per customer — CustomerDebtService upserts rather than duplicating.
        builder.HasIndex(s => s.CustomerId).IsUnique();

        // What DebtReminderJob scans every run: active schedules whose next reminder is due.
        builder.HasIndex(s => new { s.IsActive, s.NextReminderDueAt });
    }
}
