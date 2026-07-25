using System.Linq.Expressions;
using IbnAlZumar.Domain.Common;
using IbnAlZumar.Domain.Entities.Catalog;
using IbnAlZumar.Domain.Entities.Identity;
using IbnAlZumar.Domain.Entities.Inventory;
using IbnAlZumar.Domain.Entities.Purchasing;
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

    // ---- Sales ----
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<CustomerLedgerEntry> CustomerLedgerEntries => Set<CustomerLedgerEntry>();

    // ---- Identity / Dynamic RBAC ----
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserPermission> UserPermissions => Set<UserPermission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Pulls in every IEntityTypeConfiguration<T> in this assembly
        // (Configurations/Catalog, /Inventory, /Purchasing, /Sales, /Identity).
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        ApplyGlobalSoftDeleteFilter(modelBuilder);
        SeedMainWarehouse(modelBuilder);
    }

    /// <summary>
    /// Applies HasQueryFilter(e => !e.IsDeleted) to every entity that derives from BaseEntity,
    /// via reflection, so nobody has to remember to add it per-entity. Individual entity
    /// configurations intentionally do NOT set their own query filter — this is the single
    /// source of truth and must run after ApplyConfigurationsFromAssembly.
    /// </summary>
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

    /// <summary>
    /// Seeds Warehouse Id = 1 as "Main Warehouse" so Phase 1 code can safely default every
    /// Product/Order/PurchaseOrder to WarehouseId = 1 without a null check. HasData requires a
    /// static value, so CreatedAt is fixed rather than DateTime.UtcNow (which would otherwise
    /// produce a new migration every time you rebuild).
    /// </summary>
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
