// File: Persistence/Seed/DataSeeder.cs
using IbnAlZumar.API.Persistence;
using IbnAlZumar.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IbnAlZumar.Persistence.Seed;

public static class DataSeeder
{
    /// <summary>
    /// Central registry of permission codes. Controllers/policies should reference these
    /// constants (e.g. [Authorize(Policy = DataSeeder.PermissionCodes.ProductsEdit)]) instead of
    /// hardcoding strings, so a typo fails at compile time instead of silently never matching.
    /// </summary>
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

        public const string PurchasingView = "Purchasing.View";
        public const string PurchasingCreate = "Purchasing.Create";
        public const string PurchasingApprove = "Purchasing.Approve";

        public const string OrdersView = "Orders.View";
        public const string OrdersCreate = "Orders.Create";
        public const string OrdersEdit = "Orders.Edit";
        public const string OrdersCancel = "Orders.Cancel";

        public const string CustomersView = "Customers.View";
        public const string CustomersManage = "Customers.Manage";
        public const string CustomersManageDebt = "Customers.ManageDebt"; // الشكك

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
        var requiredRoles = new[] { "Admin", "Cashier" };
        var existingRoles = (await context.Roles.Select(r => r.Name).ToListAsync())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var missing = requiredRoles
            .Where(r => !existingRoles.Contains(r))
            .Select(r => new Role
            {
                Name = r,
                Description = r == "Admin" ? "Full system access" : "Point-of-sale day-to-day operations"
            })
            .ToList();

        if (missing.Count == 0) return;

        context.Roles.AddRange(missing);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} roles", missing.Count);
    }

    private static async Task SeedRolePermissionsAsync(ApplicationDbContext context, ILogger logger)
    {
        var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");
        var cashierRole = await context.Roles.FirstAsync(r => r.Name == "Cashier");
        var allPermissions = await context.Permissions.ToListAsync();

        // Admin: every permission in the system, per the requirement.
        var existingAdminIds = (await context.RolePermissions
            .Where(rp => rp.RoleId == adminRole.Id)
            .Select(rp => rp.PermissionId)
            .ToListAsync()).ToHashSet();

        var adminMissing = allPermissions
            .Where(p => !existingAdminIds.Contains(p.Id))
            .Select(p => new RolePermission { RoleId = adminRole.Id, PermissionId = p.Id })
            .ToList();

        // Cashier: a sensible default subset for day-to-day POS work. This is just seed data —
        // an Admin can add/remove permissions per-role or per-user later without any code change.
        var cashierCodes = new[]
        {
            PermissionCodes.ProductsView,
            PermissionCodes.InventoryView,
            PermissionCodes.OrdersView,
            PermissionCodes.OrdersCreate,
            PermissionCodes.CustomersView,
            PermissionCodes.CustomersManage,
        };

        var existingCashierIds = (await context.RolePermissions
            .Where(rp => rp.RoleId == cashierRole.Id)
            .Select(rp => rp.PermissionId)
            .ToListAsync()).ToHashSet();

        var cashierMissing = allPermissions
            .Where(p => cashierCodes.Contains(p.Code) && !existingCashierIds.Contains(p.Id))
            .Select(p => new RolePermission { RoleId = cashierRole.Id, PermissionId = p.Id })
            .ToList();

        if (adminMissing.Count == 0 && cashierMissing.Count == 0) return;

        context.RolePermissions.AddRange(adminMissing);
        context.RolePermissions.AddRange(cashierMissing);
        await context.SaveChangesAsync();
        logger.LogInformation(
            "Mapped {AdminCount} permissions to Admin, {CashierCount} to Cashier",
            adminMissing.Count, cashierMissing.Count);
    }

    private static async Task SeedSuperAdminAsync(ApplicationDbContext context, IPasswordHasher<User> passwordHasher, ILogger logger)
    {
        const string defaultUsername = "admin";
        const string defaultPassword = "Admin@123456"; // CHANGE IMMEDIATELY after first login.

        if (await context.Users.AnyAsync(u => u.Username == defaultUsername))
        {
            return;
        }

        var adminUser = new User
        {
            FullName = "Super Admin",
            Username = defaultUsername,
            Email = "admin@ibnalzumar.local",
            IsActive = true,
        };
        adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, defaultPassword);

        context.Users.Add(adminUser);
        await context.SaveChangesAsync(); // need adminUser.Id for the UserRole row below

        var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");
        context.UserRoles.Add(new UserRole { UserId = adminUser.Id, RoleId = adminRole.Id });
        await context.SaveChangesAsync();

        logger.LogWarning(
            "Seeded default Super Admin (username: {Username}, password: {Password}). CHANGE THE PASSWORD IMMEDIATELY.",
            defaultUsername, defaultPassword);
    }
}