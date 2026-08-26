using IbnAlZumar.Domain.Entities.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IbnAlZumar.Persistence.Configurations.Sales;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.FullName).IsRequired().HasMaxLength(150);
        builder.Property(c => c.Phone).HasMaxLength(30);
        builder.Property(c => c.Email).HasMaxLength(150);
        builder.Property(c => c.Address).HasMaxLength(300);
        builder.Property(c => c.Governorate).HasMaxLength(100);

        builder.Property(c => c.CreditLimit).HasPrecision(18, 2);
        builder.Property(c => c.CurrentBalance).HasPrecision(18, 2);

        builder.HasIndex(c => c.Phone); // not unique: walk-ins can share/omit phone numbers
    }
}

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderNumber).IsRequired().HasMaxLength(50);
        builder.Property(o => o.GuestName).HasMaxLength(150);
        builder.Property(o => o.GuestPhone).HasMaxLength(30);
        builder.Property(o => o.ShippingAddress).HasMaxLength(300);
        builder.Property(o => o.DeliveryGovernorate).HasMaxLength(100);
        builder.Property(o => o.Notes).HasMaxLength(500);

        builder.Property(o => o.Source).HasConversion<string>().HasMaxLength(20);
        builder.Property(o => o.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(o => o.PaymentMethod).HasConversion<string>().HasMaxLength(30);
        builder.Property(o => o.PaymentStatus).HasConversion<string>().HasMaxLength(30);
        builder.Property(o => o.PaymobOrderId).HasMaxLength(100);
        builder.Property(o => o.PaymobTransactionId).HasMaxLength(100);
        builder.HasIndex(o => o.PaymobOrderId);
        builder.HasIndex(o => o.PaymobTransactionId);
        builder.Property(o => o.DiscountType).HasConversion<string>().HasMaxLength(20);

        builder.Property(o => o.SubTotal).HasPrecision(18, 2);
        builder.Property(o => o.DiscountValue).HasPrecision(18, 2);
        builder.Property(o => o.DiscountAmount).HasPrecision(18, 2);
        builder.Property(o => o.TotalAmount).HasPrecision(18, 2);

        builder.HasIndex(o => o.OrderNumber).IsUnique();
        builder.HasIndex(o => o.OrderDate);

        // Every FK below is Restrict: an Order has 3 independent parents (Customer, Warehouse,
        // CashierUser). Cascading any of them risks SQL Server's multiple-cascade-path error and,
        // more importantly, orders must never disappear just because a customer/user record does.
        builder.HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Warehouse)
            .WithMany()
            .HasForeignKey(o => o.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.CashierUser)
            .WithMany()
            .HasForeignKey(o => o.CashierUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.DiscountType).HasConversion<string>().HasMaxLength(20);

        builder.Property(i => i.UnitPrice).HasPrecision(18, 2);
        builder.Property(i => i.DiscountValue).HasPrecision(18, 2);
        builder.Property(i => i.DiscountAmount).HasPrecision(18, 2);
        builder.Property(i => i.LineTotal).HasPrecision(18, 2);

        builder.HasOne(i => i.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade); // line items are true children of the order

        builder.HasOne(i => i.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Method).HasConversion<string>().HasMaxLength(30);
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(p => p.PaymobTransactionId).HasMaxLength(100);
        builder.HasIndex(p => p.PaymobTransactionId);
        builder.Property(p => p.Amount).HasPrecision(18, 2);
        builder.Property(p => p.Notes).HasMaxLength(300);

        builder.HasOne(p => p.Order)
            .WithMany(o => o.Payments)
            .HasForeignKey(p => p.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Customer)
            .WithMany(c => c.Payments)
            .HasForeignKey(p => p.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.ReceivedByUser)
            .WithMany()
            .HasForeignKey(p => p.ReceivedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class CustomerLedgerEntryConfiguration : IEntityTypeConfiguration<CustomerLedgerEntry>
{
    public void Configure(EntityTypeBuilder<CustomerLedgerEntry> builder)
    {
        builder.ToTable("CustomerLedgerEntries");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.TransactionType).HasConversion<string>().HasMaxLength(30);
        builder.Property(e => e.Amount).HasPrecision(18, 2);
        builder.Property(e => e.RunningBalance).HasPrecision(18, 2);
        builder.Property(e => e.Notes).HasMaxLength(300);

        builder.HasIndex(e => new { e.CustomerId, e.TransactionDate });

        builder.HasOne(e => e.Customer)
            .WithMany(c => c.LedgerEntries)
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.Restrict); // ledger must survive independently of Customer edits

        builder.HasOne(e => e.RelatedOrder)
            .WithMany()
            .HasForeignKey(e => e.RelatedOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.RelatedPayment)
            .WithMany()
            .HasForeignKey(e => e.RelatedPaymentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
