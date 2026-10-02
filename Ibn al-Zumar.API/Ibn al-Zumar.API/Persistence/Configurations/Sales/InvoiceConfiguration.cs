using IbnAlZumar.Domain.Entities.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IbnAlZumar.API.Persistence.Configurations.Sales;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");

        builder.HasIndex(i => i.InvoiceNumber).IsUnique();

        // Restrict everywhere: an issued invoice is a financial/fiscal record and must never
        // disappear as a side effect of deleting the order, ticket, or customer it bills.
        builder.HasOne(i => i.Order)
            .WithMany()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.MaintenanceRequest)
            .WithMany()
            .HasForeignKey(i => i.MaintenanceRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Customer)
            .WithMany()
            .HasForeignKey(i => i.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => i.OrderId);
        builder.HasIndex(i => i.MaintenanceRequestId);
    }
}
