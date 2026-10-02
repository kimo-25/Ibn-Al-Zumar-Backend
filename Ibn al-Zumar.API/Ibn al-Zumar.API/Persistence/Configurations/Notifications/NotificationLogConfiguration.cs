using IbnAlZumar.Domain.Entities.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations.Notifications;

public class NotificationLogConfiguration : IEntityTypeConfiguration<NotificationLog>
{
    public void Configure(EntityTypeBuilder<NotificationLog> builder)
    {
        builder.ToTable("NotificationLogs");

        builder.Property(n => n.Recipient).IsRequired().HasMaxLength(320);
        builder.Property(n => n.TemplateName).IsRequired().HasMaxLength(100);

        builder.HasIndex(n => new { n.RelatedEntityType, n.RelatedEntityId });
        builder.HasIndex(n => new { n.Channel, n.Status, n.CreatedAt });
    }
}
