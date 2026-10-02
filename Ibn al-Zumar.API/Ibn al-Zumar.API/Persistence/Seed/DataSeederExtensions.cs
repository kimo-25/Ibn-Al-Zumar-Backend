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

        try
        {
            await context.Database.MigrateAsync();
            await DataSeeder.SeedAsync(context, passwordHasher, logger);
        }
        catch (Exception ex)
        {
            // Log and swallow exceptions so the app can start (useful for Azure App Service startup).
            logger.LogError(ex, "Database migration or seeding failed during startup. Continuing application startup.");
        }

        return app;
    }
}