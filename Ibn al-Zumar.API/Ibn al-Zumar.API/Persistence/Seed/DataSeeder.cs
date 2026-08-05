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
        await SeedModeratorUserAsync(context, passwordHasher, logger);
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
        // Add Owner and Moderator roles so controllers that use these names match seeded roles.
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

        // Owner and Admin: every permission in the system.
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

        // Moderator: limited to Products, Categories, Customers and Orders (no Reports/Financials/Purchasing approval).
        var moderatorCodes = new[]
        {
            PermissionCodes.ProductsView,
            PermissionCodes.ProductsCreate,
            PermissionCodes.ProductsEdit,
            PermissionCodes.ProductsDelete,
            PermissionCodes.CategoriesManage,
            PermissionCodes.CustomersView,
            PermissionCodes.CustomersManage,
            PermissionCodes.OrdersView,
            PermissionCodes.OrdersCreate,
            PermissionCodes.OrdersEdit
        };

        var existingModeratorIds = (await context.RolePermissions
            .Where(rp => rp.RoleId == moderatorRole.Id)
            .Select(rp => rp.PermissionId)
            .ToListAsync()).ToHashSet();

        var moderatorMissing = allPermissions
            .Where(p => moderatorCodes.Contains(p.Code) && !existingModeratorIds.Contains(p.Id))
            .Select(p => new RolePermission { RoleId = moderatorRole.Id, PermissionId = p.Id })
            .ToList();

        // Cashier: subset used previously (POS scenario)
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

        if (ownerMissing.Count == 0 && adminMissing.Count == 0 && moderatorMissing.Count == 0 && cashierMissing.Count == 0) return;

        context.RolePermissions.AddRange(ownerMissing);
        context.RolePermissions.AddRange(adminMissing);
        context.RolePermissions.AddRange(moderatorMissing);
        context.RolePermissions.AddRange(cashierMissing);
        await context.SaveChangesAsync();
        logger.LogInformation(
            "Mapped {OwnerCount} permissions to Owner, {AdminCount} to Admin, {ModeratorCount} to Moderator, {CashierCount} to Cashier",
            ownerMissing.Count, adminMissing.Count, moderatorMissing.Count, cashierMissing.Count);
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

        // Give the seeded account the Owner role (full permissions). This ensures Super Admin/Owner
        // can access Owner Hub, Operations, Financials, and User Management.
        var ownerRole = await context.Roles.FirstAsync(r => r.Name == "Owner");
        context.UserRoles.Add(new UserRole { UserId = adminUser.Id, RoleId = ownerRole.Id });
        await context.SaveChangesAsync();

        logger.LogWarning(
            "Seeded default Super Admin (username: {Username}, password: {Password}). CHANGE THE PASSWORD IMMEDIATELY.",
            defaultUsername, defaultPassword);
    }

    private static async Task SeedModeratorUserAsync(ApplicationDbContext context, IPasswordHasher<User> passwordHasher, ILogger logger)
    {
        const string username = "Kamal";
        const string password = "Kamal2004!!"; // For testing only — remove or change in production.

        if (await context.Users.AnyAsync(u => u.Username == username))
        {
            return;
        }

        var modUser = new User
        {
            FullName = "Kamal Moderator",
            Username = username,
            Email = "kamal@local",
            IsActive = true,
        };
        modUser.PasswordHash = passwordHasher.HashPassword(modUser, password);

        context.Users.Add(modUser);
        await context.SaveChangesAsync();

        var moderatorRole = await context.Roles.FirstAsync(r => r.Name == "Moderator");
        context.UserRoles.Add(new UserRole { UserId = modUser.Id, RoleId = moderatorRole.Id });
        await context.SaveChangesAsync();

        logger.LogInformation("Seeded default Moderator (username: {Username}, password: {Password}).", username, password);
    }
}