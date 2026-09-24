using IbnAlZumar.Domain.Entities.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IbnAlZumar.API.Persistence.Configurations.Inventory;

public class ProductBatchConfiguration : IEntityTypeConfiguration<ProductBatch>
{
    public void Configure(EntityTypeBuilder<ProductBatch> builder)
    {
        // FEFO queries filter by ProductId+WarehouseId then order by ExpiryDate — composite index covers it.
        builder.HasIndex(b => new { b.ProductId, b.WarehouseId, b.ExpiryDate });
        builder.HasIndex(b => b.BatchNumber);
        builder.HasIndex(b => b.ExpiryDate); // Expiry Warning job scans across all products/warehouses

        builder.HasOne(b => b.Product)
            .WithMany(p => p.ProductBatches)
            .HasForeignKey(b => b.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Warehouse)
            .WithMany(w => w.ProductBatches)
            .HasForeignKey(b => b.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        // SupplierId is a loose reference (Supplier lives in a different module) — no FK constraint
        // here to avoid a cross-module hard dependency; validated in the service layer instead.
        builder.Property(b => b.SupplierId).IsRequired(false);
    }
}