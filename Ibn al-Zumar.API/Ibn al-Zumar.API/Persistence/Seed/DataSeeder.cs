// File: Persistence/Seed/DataSeeder.cs
using System.Globalization;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.Domain.Entities.Catalog;
using IbnAlZumar.Domain.Entities.Identity;
using IbnAlZumar.Domain.Entities.Purchasing;
using IbnAlZumar.Domain.Entities.Reminders;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IbnAlZumar.Persistence.Seed;

public static class DataSeeder
{
    /// <summary>
    /// Sheet 1: fixed, reserved Id for the "Opening Balance Supplier" (مورد أول المدة) so any
    /// module can reference it as a stable constant (e.g. ProductBatch.SupplierId, PurchaseOrder
    /// screens offering "register legacy stock") without a lookup. Never reuse this Id for a real
    /// supplier.
    /// </summary>
    public const int OpeningBalanceSupplierId = 999999;

    public static class PermissionCodes
    {
        public const string ProductsView = "Products.View";
        public const string ProductsCreate = "Products.Create";
        public const string ProductsEdit = "Products.Edit";
        public const string ProductsDelete = "Products.Delete";
        public const string CategoriesManage = "Categories.Manage";

        public const string InventoryView = "Inventory.View";
        public const string InventoryAdjust = "Inventory.Adjust";
        public const string InventoryTransfer = "Inventory.Transfer";
        public const string InventoryManageBatches = "Inventory.ManageBatches";

        public const string PurchasingView = "Purchasing.View";
        public const string PurchasingCreate = "Purchasing.Create";
        public const string PurchasingApprove = "Purchasing.Approve";

        public const string OrdersView = "Orders.View";
        public const string OrdersCreate = "Orders.Create";
        public const string OrdersEdit = "Orders.Edit";
        public const string OrdersCancel = "Orders.Cancel";

        public const string CustomersView = "Customers.View";
        public const string CustomersManage = "Customers.Manage";
        public const string CustomersManageDebt = "Customers.ManageDebt";

        public const string UsersManage = "Users.Manage";
        public const string RolesManage = "Roles.Manage";
        public const string PermissionsManage = "Permissions.Manage";

        public const string ReportsView = "Reports.View";

        public static readonly (string Code, string Name, string Module)[] All =
        {
            (ProductsView, "View Products", "Products"),
            (ProductsCreate, "Create Products", "Products"),
            (ProductsEdit, "Edit Products", "Products"),
            (ProductsDelete, "Delete Products", "Products"),
            (CategoriesManage, "Manage Categories & Brands", "Products"),
            (InventoryView, "View Inventory", "Inventory"),
            (InventoryAdjust, "Adjust Stock", "Inventory"),
            (InventoryTransfer, "Transfer Stock Between Warehouses", "Inventory"),
            (InventoryManageBatches, "Receive/Consume Product Batches", "Inventory"),
            (PurchasingView, "View Purchase Orders", "Purchasing"),
            (PurchasingCreate, "Create Purchase Orders", "Purchasing"),
            (PurchasingApprove, "Approve/Receive Purchase Orders", "Purchasing"),
            (OrdersView, "View Orders", "Sales"),
            (OrdersCreate, "Create Orders (POS/Online)", "Sales"),
            (OrdersEdit, "Edit Orders", "Sales"),
            (OrdersCancel, "Cancel Orders", "Sales"),
            (CustomersView, "View Customers", "Customers"),
            (CustomersManage, "Create/Edit Customers", "Customers"),
            (CustomersManageDebt, "Manage Customer Debt (الشكك)", "Customers"),
            (UsersManage, "Manage Users", "Administration"),
            (RolesManage, "Manage Roles", "Administration"),
            (PermissionsManage, "Manage Role/User Permissions", "Administration"),
            (ReportsView, "View Reports", "Reports"),
        };
    }

    public static async Task SeedAsync(ApplicationDbContext context, IPasswordHasher<User> passwordHasher, ILogger logger)
    {
        await SeedPermissionsAsync(context, logger);
        await SeedRolesAsync(context, logger);
        await SeedRolePermissionsAsync(context, logger);
        await SeedSuperAdminAsync(context, passwordHasher, logger);
        await SeedModeratorUserAsync(context, passwordHasher, logger);

        await SeedBrandsAsync(context, logger);
        await SeedCategoriesAsync(context, logger);
        await SeedProductsFromCsvAsync(context, logger);
        await SeedRemindersFromCsvAsync(context, logger);

        // --- Sheet 1 ---
        await SeedOpeningBalanceSupplierAsync(context, logger);
    }

    private static async Task SeedPermissionsAsync(ApplicationDbContext context, ILogger logger)
    {
        var existingCodes = (await context.Permissions.Select(p => p.Code).ToListAsync())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var missing = PermissionCodes.All
            .Where(p => !existingCodes.Contains(p.Code))
            .Select(p => new Permission { Code = p.Code, Name = p.Name, Module = p.Module })
            .ToList();

        if (missing.Count == 0) return;

        context.Permissions.AddRange(missing);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} permissions", missing.Count);
    }

    private static async Task SeedRolesAsync(ApplicationDbContext context, ILogger logger)
    {
        var requiredRoles = new[] { "Owner", "Admin", "Moderator", "Cashier" };
        var existingRoles = (await context.Roles.Select(r => r.Name).ToListAsync())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var missing = requiredRoles
            .Where(r => !existingRoles.Contains(r))
            .Select(r => new Role
            {
                Name = r,
                Description = r switch
                {
                    "Owner" => "Full system access (Owner / Super Admin)",
                    "Admin" => "Administrator with elevated privileges",
                    "Moderator" => "Limited content/data entry and management",
                    "Cashier" => "Point-of-sale day-to-day operations",
                    _ => r
                }
            })
            .ToList();

        if (missing.Count == 0) return;

        context.Roles.AddRange(missing);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} roles", missing.Count);
    }

    private static async Task SeedRolePermissionsAsync(ApplicationDbContext context, ILogger logger)
    {
        var ownerRole = await context.Roles.FirstAsync(r => r.Name == "Owner");
        var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");
        var moderatorRole = await context.Roles.FirstAsync(r => r.Name == "Moderator");
        var cashierRole = await context.Roles.FirstAsync(r => r.Name == "Cashier");
        var allPermissions = await context.Permissions.ToListAsync();

        var existingOwnerIds = (await context.RolePermissions
            .Where(rp => rp.RoleId == ownerRole.Id)
            .Select(rp => rp.PermissionId)
            .ToListAsync()).ToHashSet();

        var ownerMissing = allPermissions
            .Where(p => !existingOwnerIds.Contains(p.Id))
            .Select(p => new RolePermission { RoleId = ownerRole.Id, PermissionId = p.Id })
            .ToList();

        var existingAdminIds = (await context.RolePermissions
            .Where(rp => rp.RoleId == adminRole.Id)
            .Select(rp => rp.PermissionId)
            .ToListAsync()).ToHashSet();

        var adminMissing = allPermissions
            .Where(p => !existingAdminIds.Contains(p.Id))
            .Select(p => new RolePermission { RoleId = adminRole.Id, PermissionId = p.Id })
            .ToList();

        var moderatorCodes = new[]
        {
            PermissionCodes.ProductsView, PermissionCodes.ProductsCreate, PermissionCodes.ProductsEdit,
            PermissionCodes.ProductsDelete, PermissionCodes.CategoriesManage, PermissionCodes.CustomersView,
            PermissionCodes.CustomersManage, PermissionCodes.OrdersView, PermissionCodes.OrdersCreate, PermissionCodes.OrdersEdit
        };

        var existingModeratorIds = (await context.RolePermissions
            .Where(rp => rp.RoleId == moderatorRole.Id)
            .Select(rp => rp.PermissionId)
            .ToListAsync()).ToHashSet();

        var moderatorMissing = allPermissions
            .Where(p => moderatorCodes.Contains(p.Code) && !existingModeratorIds.Contains(p.Id))
            .Select(p => new RolePermission { RoleId = moderatorRole.Id, PermissionId = p.Id })
            .ToList();

        var cashierCodes = new[]
        {
            PermissionCodes.ProductsView, PermissionCodes.InventoryView, PermissionCodes.OrdersView,
            PermissionCodes.OrdersCreate, PermissionCodes.CustomersView, PermissionCodes.CustomersManage
        };

        var existingCashierIds = (await context.RolePermissions
            .Where(rp => rp.RoleId == cashierRole.Id)
            .Select(rp => rp.PermissionId)
            .ToListAsync()).ToHashSet();

        var cashierMissing = allPermissions
            .Where(p => cashierCodes.Contains(p.Code) && !existingCashierIds.Contains(p.Id))
            .Select(p => new RolePermission { RoleId = cashierRole.Id, PermissionId = p.Id })
            .ToList();

        var toAdd = ownerMissing.Concat(adminMissing).Concat(moderatorMissing).Concat(cashierMissing).ToList();
        if (toAdd.Count == 0) return;

        context.RolePermissions.AddRange(toAdd);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} role-permission links", toAdd.Count);
    }

    private static async Task SeedSuperAdminAsync(ApplicationDbContext context, IPasswordHasher<User> passwordHasher, ILogger logger)
    {
        const string username = "admin";
        const string password = "AdminPassword123!";

        if (await context.Users.AnyAsync(u => u.Username == username)) return;

        var adminUser = new User
        {
            FullName = "Super Admin Owner",
            Username = username,
            Email = "admin@ibnalzumar.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, password);

        context.Users.Add(adminUser);
        await context.SaveChangesAsync();

        var ownerRole = await context.Roles.FirstAsync(r => r.Name == "Owner");
        context.UserRoles.Add(new UserRole { UserId = adminUser.Id, RoleId = ownerRole.Id });
        await context.SaveChangesAsync();

        logger.LogInformation("Seeded Super Admin Owner user.");
    }

    private static async Task SeedModeratorUserAsync(ApplicationDbContext context, IPasswordHasher<User> passwordHasher, ILogger logger)
    {
        const string username = "kamal";
        const string password = "Kamal2004!!";

        if (await context.Users.AnyAsync(u => u.Username == username)) return;

        var modUser = new User
        {
            FullName = "Kamal Moderator",
            Username = username,
            Email = "kamal@local",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        modUser.PasswordHash = passwordHasher.HashPassword(modUser, password);

        context.Users.Add(modUser);
        await context.SaveChangesAsync();

        var moderatorRole = await context.Roles.FirstAsync(r => r.Name == "Moderator");
        context.UserRoles.Add(new UserRole { UserId = modUser.Id, RoleId = moderatorRole.Id });
        await context.SaveChangesAsync();

        logger.LogInformation("Seeded default Moderator (Kamal).");
    }

    private static async Task SeedBrandsAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.Brands.AnyAsync()) return;

        context.Brands.Add(new Brand
        {
            Name = "JADEVER",
            LogoUrl = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        });

        await context.SaveChangesAsync();
        logger.LogInformation("Seeded Brands successfully.");
    }

    private static async Task SeedCategoriesAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.Categories.AnyAsync()) return;

        context.Categories.AddRange(
            new Category
            {
                Name = "الأدوات والأجهزة",
                NameAr = "الأدوات والأجهزة",
                Slug = "الأدوات-والأجهزة",
                ParentCategoryId = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Category
            {
                Name = "أدوات الديكور",
                NameAr = "أدوات الديكور",
                Slug = "أدوات-الديكور",
                ParentCategoryId = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );

        await context.SaveChangesAsync();
        logger.LogInformation("Seeded Categories successfully.");
    }

    private static async Task SeedProductsFromCsvAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.Products.AnyAsync()) return;

        var path = GetFilePath("Products.csv");
        if (!File.Exists(path))
        {
            logger.LogWarning("Products.csv file not found at path {Path}", path);
            return;
        }

        var lines = await File.ReadAllLinesAsync(path);
        if (lines.Length <= 1) return;

        var defaultBrand = await context.Brands.FirstOrDefaultAsync();
        var defaultCategory = await context.Categories.FirstOrDefaultAsync();

        if (defaultBrand == null || defaultCategory == null)
        {
            logger.LogWarning("Cannot seed products: Brands or Categories are missing.");
            return;
        }

        var products = new List<Product>();
        var existingSkus = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var line in lines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split('\t').Length > 1 ? line.Split('\t') : line.Split(',');
            if (parts.Length < 6) continue;

            try
            {
                var rawSku = GetValue(parts, 1);
                var sku = !string.IsNullOrWhiteSpace(rawSku) ? rawSku : Guid.NewGuid().ToString()[..8].ToUpper();

                if (existingSkus.Contains(sku))
                {
                    sku = $"{sku}-{Guid.NewGuid().ToString()[..4].ToUpper()}";
                }

                existingSkus.Add(sku);

                var product = new Product
                {
                    SKU = sku,
                    Name = GetValue(parts, 3) ?? "Product",
                    NameAr = GetValue(parts, 4),
                    Description = GetValue(parts, 5),
                    SellingPrice = parseDecimal(GetValue(parts, 6)),
                    CurrentCostPrice = parseDecimal(GetValue(parts, 7)),
                    QuantityPerCarton = parseInt(GetValue(parts, 8), 1),
                    IsActive = parseBool(GetValue(parts, 9)),
                    TrackInventory = parseBool(GetValue(parts, 10)),
                    CategoryId = defaultCategory.Id,
                    BrandId = defaultBrand.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };
                products.Add(product);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to parse product row: {Line}", line);
            }
        }

        if (products.Count > 0)
        {
            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
            logger.LogInformation("Successfully seeded {Count} products into SQL Server.", products.Count);
        }
    }

    private static async Task SeedRemindersFromCsvAsync(ApplicationDbContext context, ILogger logger)
    {
        if (!await context.Set<Reminder>().AnyAsync())
        {
            var path = GetFilePath("Reminders.csv");
            if (File.Exists(path))
            {
                logger.LogInformation("Reminders.csv file found, processing...");
            }
        }
    }

    private static async Task SeedOpeningBalanceSupplierAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.Set<Supplier>().AnyAsync(s => s.Id == OpeningBalanceSupplierId))
            return;

        var supplier = new Supplier
        {
            Id = OpeningBalanceSupplierId,
            Name = "Opening Balance Supplier (مورد أول المدة)",
            ContactPerson = null,
            Phone = null,
            Email = null,
            Address = null,
            TaxId = "OPENING-BALANCE",
            CurrentBalance = 0m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await using var dbTransaction = await context.Database.BeginTransactionAsync();
        try
        {
            // استخدام ExecuteSqlRawAsync مع نصوص ثابتة بدون Interpolation لحل الـ Warnings تماماً
            await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Suppliers ON");
            context.Set<Supplier>().Add(supplier);
            await context.SaveChangesAsync();
            await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Suppliers OFF");
            await dbTransaction.CommitAsync();

            logger.LogInformation("Seeded Opening Balance Supplier (مورد أول المدة) with fixed Id={Id}", OpeningBalanceSupplierId);
        }
        catch (Exception ex)
        {
            await dbTransaction.RollbackAsync();
            logger.LogError(ex, "Failed to seed Opening Balance Supplier with fixed Id={Id}", OpeningBalanceSupplierId);
            throw;
        }
    }

    private static string GetFilePath(string fileName)
    {
        var basePath = AppContext.BaseDirectory;
        var directPath = Path.Combine(basePath, "Seed", fileName);
        if (File.Exists(directPath)) return directPath;

        var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "Persistence", "Seed", fileName);
        if (File.Exists(rootPath)) return rootPath;

        return Path.Combine(Directory.GetCurrentDirectory(), fileName);
    }

    private static string? GetValue(string[] parts, int index) =>
        index < parts.Length && !string.IsNullOrWhiteSpace(parts[index]) && parts[index] != "NULL"
            ? parts[index].Trim('"', ' ')
            : null;

    private static decimal parseDecimal(string? val) =>
        decimal.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out var res) ? res : 0m;

    private static int parseInt(string? val, int defaultVal = 0) =>
        int.TryParse(val, out var res) ? res : defaultVal;

    private static bool parseBool(string? val) =>
        val == "1" || string.Equals(val, "true", StringComparison.OrdinalIgnoreCase);
}