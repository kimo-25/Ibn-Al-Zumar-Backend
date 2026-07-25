using IbnAlZumar.Domain.Entities.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IbnAlZumar.Persistence.Configurations.Inventory;

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable("Warehouses");
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Name).IsRequired().HasMaxLength(150);
        builder.Property(w => w.Address).HasMaxLength(300);

        builder.HasIndex(w => w.Name).IsUnique();
    }
}

public class ProductStockConfiguration : IEntityTypeConfiguration<ProductStock>
{
    public void Configure(EntityTypeBuilder<ProductStock> builder)
    {
        builder.ToTable("ProductStocks");
        builder.HasKey(s => s.Id);

        // One stock row per (Product, Warehouse) pair.
        builder.HasIndex(s => new { s.ProductId, s.WarehouseId }).IsUnique();

        builder.HasOne(s => s.Product)
            .WithMany(p => p.Stocks)
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Warehouse)
            .WithMany(w => w.ProductStocks)
            .HasForeignKey(s => s.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
{
    public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
    {
        builder.ToTable("InventoryTransactions");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.TransactionType).HasConversion<string>().HasMaxLength(30);
        builder.Property(t => t.ReferenceType).HasMaxLength(50);
        builder.Property(t => t.Notes).HasMaxLength(500);

        // The audit trail must survive even if the product or warehouse record changes elsewhere.
        builder.HasOne(t => t.Product)
            .WithMany()
            .HasForeignKey(t => t.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Warehouse)
            .WithMany()
            .HasForeignKey(t => t.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => new { t.ProductId, t.WarehouseId, t.TransactionDate });
    }
}

public class StockTransferConfiguration : IEntityTypeConfiguration<StockTransfer>
{
    public void Configure(EntityTypeBuilder<StockTransfer> builder)
    {
        builder.ToTable("StockTransfers");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.Notes).HasMaxLength(500);

        builder.HasOne(t => t.SourceWarehouse)
            .WithMany(w => w.OutgoingTransfers)
            .HasForeignKey(t => t.SourceWarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.DestinationWarehouse)
            .WithMany(w => w.IncomingTransfers)
            .HasForeignKey(t => t.DestinationWarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class StockTransferItemConfiguration : IEntityTypeConfiguration<StockTransferItem>
{
    public void Configure(EntityTypeBuilder<StockTransferItem> builder)
    {
        builder.ToTable("StockTransferItems");
        builder.HasKey(i => i.Id);

        builder.HasOne(i => i.StockTransfer)
            .WithMany(t => t.Items)
            .HasForeignKey(i => i.StockTransferId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
