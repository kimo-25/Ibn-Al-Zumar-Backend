using IbnAlZumar.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IbnAlZumar.API.Persistence.Configurations.Catalog
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.SKU)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(p => p.SKU)
                .IsUnique();

            builder.Property(p => p.Barcode)
                .HasMaxLength(50);

            builder.HasIndex(p => p.Barcode);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.NameAr)
                .HasMaxLength(200);

            builder.Property(p => p.SellingPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.CurrentCostPrice)
                .HasColumnType("decimal(18,2)");

            builder.HasIndex(p => p.CategoryId);
            builder.HasIndex(p => p.BrandId);

            builder.HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Brand)
                .WithMany()
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}