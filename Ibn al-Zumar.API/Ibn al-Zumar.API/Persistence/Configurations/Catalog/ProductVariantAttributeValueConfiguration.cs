using IbnAlZumar.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IbnAlZumar.API.Persistence.Configurations.Catalog;

public class ProductVariantAttributeValueConfiguration : IEntityTypeConfiguration<ProductVariantAttributeValue>
{
    public void Configure(EntityTypeBuilder<ProductVariantAttributeValue> builder)
    {
        builder.HasOne(v => v.ProductVariant)
            .WithMany(pv => pv.AttributeValues)
            .HasForeignKey(v => v.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(v => v.ProductAttributeDefinition)
            .WithMany(d => d.ProductVariantAttributeValues)
            .HasForeignKey(v => v.ProductAttributeDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(v => new { v.ProductVariantId, v.ProductAttributeDefinitionId }).IsUnique();
    }
}