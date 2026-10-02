using IbnAlZumar.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IbnAlZumar.API.Persistence.Configurations.Catalog;

public class ProductPriceConfiguration : IEntityTypeConfiguration<ProductPrice>
{
    public void Configure(EntityTypeBuilder<ProductPrice> builder)
    {
        builder.ToTable("ProductPrices");

        builder.HasOne(pp => pp.Product)
            .WithMany(p => p.Prices)
            .HasForeignKey(pp => pp.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Restrict (not Cascade) here: Product already cascades to ProductVariant, and
        // ProductVariant would also cascade to ProductPrice if this were Cascade too —
        // SQL Server rejects that as "multiple cascade paths". Deleting a product still
        // removes its prices via the Product->ProductPrice path above; deleting a single
        // variant while keeping the product must not also silently delete price rows tied
        // to other variants, so Restrict is also the semantically safer choice here.
        builder.HasOne(pp => pp.ProductVariant)
            .WithMany(v => v.Prices)
            .HasForeignKey(pp => pp.ProductVariantId)
            .OnDelete(DeleteBehavior.Restrict);

        // Prevents two identical price-break rows (same product/variant/tier/quantity).
        builder.HasIndex(pp => new { pp.ProductId, pp.ProductVariantId, pp.Tier, pp.MinQuantity })
            .IsUnique();
    }
}
