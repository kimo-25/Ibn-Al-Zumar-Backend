using IbnAlZumar.Domain.Entities.Maintenance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IbnAlZumar.API.Persistence.Configurations.Maintenance;

public class MaintenancePartUsageConfiguration : IEntityTypeConfiguration<MaintenancePartUsage>
{
    public void Configure(EntityTypeBuilder<MaintenancePartUsage> builder)
    {
        builder.ToTable("MaintenancePartUsages");

        builder.HasOne(u => u.MaintenanceRequest)
            .WithMany(m => m.PartUsages)
            .HasForeignKey(u => u.MaintenanceRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        // Restrict everywhere else: a part-usage row is a financial/inventory record and must
        // never disappear as a side effect of deleting the product, warehouse, or the
        // InventoryTransaction it produced.
        builder.HasOne(u => u.Product)
            .WithMany()
            .HasForeignKey(u => u.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.Warehouse)
            .WithMany()
            .HasForeignKey(u => u.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.InventoryTransaction)
            .WithMany()
            .HasForeignKey(u => u.InventoryTransactionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
