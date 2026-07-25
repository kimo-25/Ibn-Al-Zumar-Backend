// File: Program.cs
using IbnAlZumar.Api.Authorization;
using IbnAlZumar.Api.Common.Settings;
using IbnAlZumar.Api.Middleware;
using IbnAlZumar.Api.Services.Auth;
using IbnAlZumar.Api.Services.Catalog;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.API.Services.Catalog;
using IbnAlZumar.API.Services.Customers;
using IbnAlZumar.API.Services.Identity;
using IbnAlZumar.API.Services.Inventory;
using IbnAlZumar.API.Services.Purchasing;
using IbnAlZumar.Domain.Entities.Identity;
using IbnAlZumar.Persistence;
using IbnAlZumar.Persistence.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// Configuration
// ---------------------------------------------------------------------------
var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
    ?? throw new InvalidOperationException("Missing 'Jwt' configuration section in appsettings.json.");

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

var allowedCorsOrigins = builder.Configuration.GetSection("AllowedCorsOrigins").Get<string[]>()
    ?? Array.Empty<string>();

// ---------------------------------------------------------------------------
// DbContext
// ---------------------------------------------------------------------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// ---------------------------------------------------------------------------
// Password hashing — Identity's PasswordHasher<TUser> used standalone (PBKDF2 under the hood),
// no full ASP.NET Core Identity system / Identity tables involved.
// ---------------------------------------------------------------------------
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

// ---------------------------------------------------------------------------
// Application services
// ---------------------------------------------------------------------------
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddScoped<IInventoryService, InventoryService>();

builder.Services.AddScoped<IPurchasingService, PurchasingService>();

builder.Services.AddScoped<IUserManagementService, UserManagementService>();

// ---------------------------------------------------------------------------
// Authentication (JWT Bearer)
// ---------------------------------------------------------------------------
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,

        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),

        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});

// ---------------------------------------------------------------------------
// Authorization — dynamic permission-based policies (see Authorization/ folder)
// ---------------------------------------------------------------------------
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddAuthorization();

// ---------------------------------------------------------------------------
// CORS — local POS frontend (React/Vite dev server, etc.)
// ---------------------------------------------------------------------------
const string CorsPolicyName = "PosFrontend";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        policy.WithOrigins(allowedCorsOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ---------------------------------------------------------------------------
// Controllers
// ---------------------------------------------------------------------------
builder.Services.AddControllers();

// ---------------------------------------------------------------------------
// Swagger with Bearer token support
// ---------------------------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Ibn Al-Zumar API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste just the raw token here — Swagger prefixes 'Bearer ' automatically."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ---------------------------------------------------------------------------
// Middleware pipeline — order matters
// ---------------------------------------------------------------------------

// 1. Exception handling first, so it can catch anything thrown by everything below it.
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 2. CORS before auth, so preflight requests aren't blocked by the auth pipeline.
app.UseCors(CorsPolicyName);

// 3. AuthN before AuthZ, always.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ---------------------------------------------------------------------------
// Seed database: applies pending migrations, then Roles/Permissions/Super Admin.
// ---------------------------------------------------------------------------
await app.SeedDatabaseAsync();

app.Run();
//{
//"username": "admin",
//  "password": "Admin@123456"
//}