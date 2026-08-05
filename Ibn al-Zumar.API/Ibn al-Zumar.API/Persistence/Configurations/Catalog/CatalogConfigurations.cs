using IbnAlZumar.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IbnAlZumar.Persistence.Configurations.Catalog;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(150);
        builder.Property(c => c.NameAr).HasMaxLength(150);
        builder.Property(c => c.Slug).IsRequired().HasMaxLength(160);

        builder.HasIndex(c => c.Slug).IsUnique();

        builder.HasOne(c => c.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("Brands");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name).IsRequired().HasMaxLength(100);
        builder.Property(b => b.LogoUrl).HasMaxLength(500);

        builder.HasIndex(b => b.Name).IsUnique();
    }
}

public class ProductAttributeDefinitionConfiguration : IEntityTypeConfiguration<ProductAttributeDefinition>
{
    public void Configure(EntityTypeBuilder<ProductAttributeDefinition> builder)
    {
        builder.ToTable("ProductAttributeDefinitions");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Unit).HasMaxLength(20);
        builder.Property(a => a.DataType).HasConversion<string>().HasMaxLength(20);

        builder.HasIndex(a => a.Name).IsUnique();
    }
}

public class ProductAttributeValueConfiguration : IEntityTypeConfiguration<ProductAttributeValue>
{
    public void Configure(EntityTypeBuilder<ProductAttributeValue> builder)
    {
        builder.ToTable("ProductAttributeValues");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Value).IsRequired().HasMaxLength(200);

        builder.HasIndex(v => new { v.ProductId, v.ProductAttributeDefinitionId }).IsUnique();

        builder.HasOne(v => v.Product)
            .WithMany(p => p.AttributeValues)
            .HasForeignKey(v => v.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(v => v.ProductAttributeDefinition)
            .WithMany(a => a.ProductAttributeValues)
            .HasForeignKey(v => v.ProductAttributeDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("ProductImages");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.ImageUrl).IsRequired().HasMaxLength(500);

        builder.HasOne(i => i.Product)
            .WithMany(p => p.Images)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.SKU).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Barcode).HasMaxLength(50);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(300);
        builder.Property(p => p.NameAr).HasMaxLength(300);

        builder.Property(p => p.SellingPrice).HasPrecision(18, 2);
        builder.Property(p => p.CurrentCostPrice).HasPrecision(18, 2);

        // Map scalar ImageUrl column (optional)
        builder.Property(p => p.ImageUrl).HasMaxLength(500);

        builder.HasIndex(p => p.SKU).IsUnique();
        builder.HasIndex(p => p.Barcode);
        builder.HasIndex(p => p.CategoryId);
        builder.HasIndex(p => p.BrandId);

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Brand)
            .WithMany(b => b.Products)
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

// NEW
public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("ProductVariants");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.SKU).IsRequired().HasMaxLength(100);
        builder.Property(v => v.Price).HasPrecision(18, 2);
        builder.Property(v => v.StockQuantity).IsRequired();
        builder.Property(v => v.Color).HasMaxLength(100);
        builder.Property(v => v.Finish).HasMaxLength(100);
        builder.Property(v => v.Material).HasMaxLength(100);

        builder.HasIndex(v => v.SKU).IsUnique();
        builder.HasIndex(v => new { v.ProductId, v.IsActive });

        builder.HasOne(v => v.Product)
            .WithMany(p => p.Variants)
            .HasForeignKey(v => v.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}