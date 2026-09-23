// File: Persistence/Seed/DataSeederExtensions.cs
using IbnAlZumar.API.Persistence;
using IbnAlZumar.Domain.Entities.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IbnAlZumar.Persistence.Seed;

public static class DataSeederExtensions
{
    /// <summary>
    /// Call once at startup (after app = builder.Build(), before app.Run()) to apply pending
    /// migrations and ensure Roles/Permissions/Super Admin exist. Safe to call on every startup —
    /// every seed step checks for existing data first.
    /// </summary>
    public static async Task<WebApplication> SeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = services.GetRequiredService<IPasswordHasher<User>>();
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DataSeeder");

        // --- تطبيق الـ Migrations (مع حماية: لا يتوقف الإقلاع لو فشلت الـ Migrations) ---
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to apply database migrations on startup. Startup will continue.");
            return app;
        }

        // --- تنفيذ خطوات الـ Seeding واحدة تلو الأخرى حتى لا يُسبب أي خطأ في CSV إيقاف التطبيق ---
        await RunSeedStepAsync(logger, "SeedPermissions", () => DataSeeder.SeedPermissionsAsync(context, logger));
        await RunSeedStepAsync(logger, "SeedRoles", () => DataSeeder.SeedRolesAsync(context, logger));
        await RunSeedStepAsync(logger, "SeedRolePermissions", () => DataSeeder.SeedRolePermissionsAsync(context, logger));
        await RunSeedStepAsync(logger, "SeedSuperAdmin", () => DataSeeder.SeedSuperAdminAsync(context, passwordHasher, logger));
        await RunSeedStepAsync(logger, "SeedModeratorUser", () => DataSeeder.SeedModeratorUserAsync(context, passwordHasher, logger));
        await RunSeedStepAsync(logger, "SeedBrands", () => DataSeeder.SeedBrandsAsync(context, logger));
        await RunSeedStepAsync(logger, "SeedCategories", () => DataSeeder.SeedCategoriesAsync(context, logger));
        await RunSeedStepAsync(logger, "SeedProductsFromCsv", () => DataSeeder.SeedProductsFromCsvAsync(context, logger));
        await RunSeedStepAsync(logger, "SeedRemindersFromCsv", () => DataSeeder.SeedRemindersFromCsvAsync(context, logger));

        return app;
    }

    /// <summary>
    /// يغلّف خطوة seeding واحدة بـ try-catch ويسجّل الأخطاء لضمان استمرار الإقلاع بأمان حتى لو فشلت خطوة معينة.
    /// </summary>
    private static async Task RunSeedStepAsync(ILogger logger, string stepName, Func<Task> seedStep)
    {
        try
        {
            await seedStep();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Seeding step '{Step}' failed. Application startup will continue safely.", stepName);
        }
    }
}