using IbnAlZumar.Domain.Entities.Maintenance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IbnAlZumar.API.Persistence.Configurations.Maintenance;

public class MaintenanceNoteConfiguration : IEntityTypeConfiguration<MaintenanceNote>
{
    public void Configure(EntityTypeBuilder<MaintenanceNote> builder)
    {
        builder.ToTable("MaintenanceNotes");

        builder.Property(n => n.Note).IsRequired();

        builder.HasOne(n => n.MaintenanceRequest)
            .WithMany(m => m.Notes)
            .HasForeignKey(n => n.MaintenanceRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        // Restrict: a note is an audit-trail entry — it must survive even if the authoring
        // user account is later deleted/deactivated.
        builder.HasOne(n => n.AuthorUser)
            .WithMany()
            .HasForeignKey(n => n.AuthorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(n => new { n.MaintenanceRequestId, n.CreatedAt });
    }
}
