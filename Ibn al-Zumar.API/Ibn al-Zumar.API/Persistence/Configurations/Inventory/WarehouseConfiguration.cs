using IbnAlZumar.Domain.Entities.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IbnAlZumar.API.Persistence.Configurations.Inventory;

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        // Self-referencing hierarchy: MainCentral -> RegionalBranch -> PosShelfLocation.
        // Restrict delete so a parent warehouse can't be removed while children still reference it.
        builder.HasOne(w => w.ParentWarehouse)
            .WithMany(w => w.ChildWarehouses)
            .HasForeignKey(w => w.ParentWarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(w => w.Tier);
        builder.HasIndex(w => w.ParentWarehouseId);
    }
}