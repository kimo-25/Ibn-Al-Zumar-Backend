using System.Linq.Expressions;
using IbnAlZumar.Domain.Common;
using IbnAlZumar.Domain.Entities.Attendance;
using IbnAlZumar.Domain.Entities.Ai;
using IbnAlZumar.Domain.Entities.Catalog;
using IbnAlZumar.Domain.Entities.Identity;
using IbnAlZumar.Domain.Entities.Inventory;
using IbnAlZumar.Domain.Entities.Purchasing;
using IbnAlZumar.Domain.Entities.Reminders;
using IbnAlZumar.Domain.Entities.Sales;
using Microsoft.EntityFrameworkCore;

namespace IbnAlZumar.API.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // ---- Catalog ----
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductAttributeDefinition> ProductAttributeDefinitions => Set<ProductAttributeDefinition>();
    public DbSet<ProductAttributeValue> ProductAttributeValues => Set<ProductAttributeValue>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();

    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();

    // ---- Inventory ----
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<ProductStock> ProductStocks => Set<ProductStock>();
    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();
    public DbSet<StockTransfer> StockTransfers => Set<StockTransfer>();
    public DbSet<StockTransferItem> StockTransferItems => Set<StockTransferItem>();

    // ---- Purchasing ----
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();

    // ---- Purchasing / Supplier Accounting ----
    public DbSet<SupplierPayment> SupplierPayments => Set<SupplierPayment>();
    public DbSet<SupplierLedgerEntry> SupplierLedgerEntries => Set<SupplierLedgerEntry>();

    // ---- Sales ----
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<CustomerLedgerEntry> CustomerLedgerEntries => Set<CustomerLedgerEntry>();

    // ---- Shipping Zones ----
    public DbSet<ShippingZone> ShippingZones => Set<ShippingZone>();

    // ---- Identity / Dynamic RBAC ----
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserPermission> UserPermissions => Set<UserPermission>();

    // ---- Reminders ----
    public DbSet<Reminder> Reminders => Set<Reminder>();

    // ---- Maintenance ----
    public DbSet<IbnAlZumar.Domain.Entities.Maintenance.MaintenanceRequest> MaintenanceRequests => Set<IbnAlZumar.Domain.Entities.Maintenance.MaintenanceRequest>();

    // ---- Attendance & Payroll (Voice Biometric Attendance) ----
    public DbSet<AttendanceLog> AttendanceLogs => Set<AttendanceLog>();
    public DbSet<PayrollRecord> PayrollRecords => Set<PayrollRecord>();

    // ---- AI audit trail ----
    public DbSet<AiAuditLog> AiAuditLogs => Set<AiAuditLog>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        // ��� ����� ������� ����� ���� decimal ����� ��������� ���� �������
        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.Entity<AiAuditLog>(entity =>
        {
            entity.ToTable("AiAuditLogs");
            entity.HasKey(log => log.Id);
            entity.Property(log => log.UserEmail).HasMaxLength(320);
            entity.Property(log => log.Roles).HasMaxLength(1000).IsRequired();
            entity.Property(log => log.Action).HasMaxLength(64).IsRequired();
            entity.Property(log => log.ToolName).HasMaxLength(128);
            entity.Property(log => log.IpAddress).HasMaxLength(64);
            entity.HasIndex(log => log.TimestampUtc);
            entity.HasIndex(log => new { log.UserId, log.TimestampUtc });
            entity.HasIndex(log => log.ToolName);
        });

        // ClientUuid Unique Filtered Index for Offline Sync Idempotency (SQL Server standard filter)
        modelBuilder.Entity<Order>()
            .HasIndex(o => o.ClientUuid)
            .IsUnique()
            .HasFilter("[ClientUuid] IS NOT NULL");

        modelBuilder.Entity<AttendanceLog>()
            .HasOne(a => a.User)
            .WithMany(u => u.AttendanceLogs)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AttendanceLog>()
            .HasIndex(a => new { a.UserId, a.CheckInTime });

        modelBuilder.Entity<PayrollRecord>()
            .HasOne(p => p.User)
            .WithMany(u => u.PayrollRecords)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // ---- Supplier Accounting (Ledger & Payments) ----

        modelBuilder.Entity<SupplierPayment>()
            .HasOne(sp => sp.Supplier)
            .WithMany(s => s.Payments)
            .HasForeignKey(sp => sp.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SupplierPayment>()
            .HasOne(sp => sp.PurchaseOrder)
            .WithMany()
            .HasForeignKey(sp => sp.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SupplierPayment>()
            .HasOne(sp => sp.CreatedByUser)
            .WithMany()
            .HasForeignKey(sp => sp.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SupplierPayment>()
            .HasIndex(sp => new { sp.SupplierId, sp.PaymentDate });

        modelBuilder.Entity<SupplierLedgerEntry>()
            .HasOne(sl => sl.Supplier)
            .WithMany(s => s.LedgerEntries)
            .HasForeignKey(sl => sl.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SupplierLedgerEntry>()
            .HasOne(sl => sl.RelatedPurchaseOrder)
            .WithMany()
            .HasForeignKey(sl => sl.RelatedPurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SupplierLedgerEntry>()
            .HasOne(sl => sl.RelatedPayment)
            .WithMany(sp => sp.LedgerEntries)
            .HasForeignKey(sl => sl.RelatedPaymentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SupplierLedgerEntry>()
            .HasIndex(sl => new { sl.SupplierId, sl.TransactionDate });

        ApplyGlobalSoftDeleteFilter(modelBuilder);

        // �� ����� EF Core 10622 ����� �������� �������� �������� �� Joint Tables
        modelBuilder.Entity<RolePermission>()
            .HasQueryFilter(rp => !rp.Permission.IsDeleted);

        modelBuilder.Entity<UserPermission>()
            .HasQueryFilter(up => !up.Permission.IsDeleted);

        modelBuilder.Entity<UserRole>()
            .HasQueryFilter(ur => !ur.Role.IsDeleted);

        SeedMainWarehouse(modelBuilder);
    }

    private static void ApplyGlobalSoftDeleteFilter(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                continue;

            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
            var condition = Expression.Equal(property, Expression.Constant(false));
            var lambda = Expression.Lambda(condition, parameter);

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
        }
    }

    private static void SeedMainWarehouse(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Warehouse>().HasData(new Warehouse
        {
            Id = 1,
            Name = "Main Warehouse",
            Address = null,
            IsMainWarehouse = true,
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = null,
            IsDeleted = false
        });
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}