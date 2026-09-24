// File: Program.cs
using IbnAlZumar.Api.Authorization;
using IbnAlZumar.Api.Common.Settings;
using IbnAlZumar.Api.Middleware;
using IbnAlZumar.Api.Services.Auth;
using IbnAlZumar.Api.Services.Catalog;
using IbnAlZumar.Api.Services.Email;
using IbnAlZumar.API.Ai;
using IbnAlZumar.API.Ai.Files;
using IbnAlZumar.API.Ai.Tools;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.API.Services.Ai;
using IbnAlZumar.API.Services.Attendance;
using IbnAlZumar.API.Services.Catalog;
using IbnAlZumar.API.Services.Customers;
using IbnAlZumar.API.Services.Identity;
using IbnAlZumar.API.Services.Inventory;
using IbnAlZumar.API.Services.Purchasing;
using IbnAlZumar.API.Services.Payments;
using IbnAlZumar.API.Services.Reminders;
using IbnAlZumar.API.Services.Sales;
using IbnAlZumar.Domain.Entities.Identity;
using IbnAlZumar.Persistence;
using IbnAlZumar.Persistence.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Services.Sales;
using System.Net;
using System.Net.Sockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// Configure URLs for Container hosting / Azure
// ---------------------------------------------------------------------------
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
{
    builder.WebHost.UseUrls($"http://*:{port}");
}

// ---------------------------------------------------------------------------
// Configuration (JWT Settings — NO hardcoded fallback. C-07)
// ---------------------------------------------------------------------------
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? builder.Configuration["Jwt__Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "JWT signing key is missing. Set 'Jwt:Key' in configuration (appsettings, " +
        "environment variable 'Jwt__Key', Azure App Settings, or a secret store) before starting the application.");
}

if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
{
    throw new InvalidOperationException(
        "JWT signing key ('Jwt:Key') must be at least 256 bits (32 bytes) long. " +
        $"The configured key is only {Encoding.UTF8.GetByteCount(jwtKey)} bytes.");
}

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? builder.Configuration["Jwt__Issuer"]
    ?? "IbnAlZumarApi";

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? builder.Configuration["Jwt__Audience"]
    ?? "IbnAlZumarApp";

var jwtSettings = new JwtSettings
{
    Key = jwtKey,
    Issuer = jwtIssuer,
    Audience = jwtAudience
};

builder.Services.Configure<JwtSettings>(options =>
{
    options.Key = jwtSettings.Key;
    options.Issuer = jwtSettings.Issuer;
    options.Audience = jwtSettings.Audience;
});

// Brevo Email Settings
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("Brevo"));

// Gemini AI Settings
builder.Services.Configure<GeminiSettings>(builder.Configuration.GetSection(GeminiSettings.SectionName));
builder.Services.Configure<PaymobOptions>(builder.Configuration.GetSection("Paymob"));

// ---------------------------------------------------------------------------
// DbContext (SQL Server Configuration)
// ---------------------------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("SQLAZURECONNSTR_DefaultConnection")
    ?? Environment.GetEnvironmentVariable("DATABASE_URL");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Database connection string is missing.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// ---------------------------------------------------------------------------
// Password hashing — Standalone PasswordHasher<User>
// ---------------------------------------------------------------------------
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

// ---------------------------------------------------------------------------
// Application services
// ---------------------------------------------------------------------------
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAiAuditLogService, AiAuditLogService>();
builder.Services.AddScoped<IInvoiceToExcelService, InvoiceToExcelService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();

builder.Services.AddScoped<IbnAlZumar.API.Services.Purchasing.IPurchasingService, IbnAlZumar.API.Services.Purchasing.PurchasingService>();

builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddHttpClient<IPaymobService, PaymobService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Reminders Service Registration
builder.Services.AddScoped<IReminderService, ReminderService>();

// Email Service Registration
builder.Services.AddScoped<IEmailService, EmailService>();

// ---------------------------------------------------------------------------
// AI Assistant (Gemini) Integration & Multimodal Processing
// ---------------------------------------------------------------------------
builder.Services.AddScoped<IAiFileProcessingService, AiFileProcessingService>();

builder.Services.AddHttpClient<IAiAssistantService, AiAssistantService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Register AI Tools & Registry (Basic + Catalog + Excel Tools)
builder.Services.AddSingleton<IAiTool, GetPendingOrdersTool>();
builder.Services.AddSingleton<IAiTool, GetOrderDetailsTool>();
builder.Services.AddSingleton<IAiTool, GetLowStockProductsTool>();
builder.Services.AddSingleton<IAiTool, GetSalesSummaryTool>();
builder.Services.AddSingleton<IAiTool, UpdateProductPriceTool>();

// New Catalog & Excel Tools
builder.Services.AddSingleton<IAiTool, GetCategoriesTool>();
builder.Services.AddSingleton<IAiTool, CreateCategoryTool>();
builder.Services.AddSingleton<IAiTool, CreateProductTool>();
builder.Services.AddSingleton<IAiTool, BulkImportProductsTool>();
builder.Services.AddSingleton<IAiTool, GenerateProductsExcelTool>();

// AI Tool Registry
builder.Services.AddSingleton<AiToolRegistry>();

// ---------------------------------------------------------------------------
// Voice Biometrics & Commands
// ---------------------------------------------------------------------------
builder.Services.AddScoped<IVoiceVerificationService, VoiceVerificationService>();
builder.Services.AddScoped<IVoiceCommandService, VoiceCommandService>();

builder.Services.AddScoped<IAttendanceService, AttendanceService>();

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
// Authorization
// ---------------------------------------------------------------------------
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddAuthorization();

// ---------------------------------------------------------------------------
// CORS Policy Configuration
// ---------------------------------------------------------------------------
const string CorsPolicyName = "PosFrontend";

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        policy.SetIsOriginAllowed(origin =>
                    string.IsNullOrEmpty(origin) ||
                    origin.StartsWith("https://kimo-25.github.io") ||
                    origin.StartsWith("http://localhost"))
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// ---------------------------------------------------------------------------
// Controllers
// ---------------------------------------------------------------------------
builder.Services.AddControllers();

// ---------------------------------------------------------------------------
// Swagger
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
// Auto-Apply Migrations & Seed database
// ---------------------------------------------------------------------------
await app.SeedDatabaseAsync();

// ---------------------------------------------------
// Middleware pipeline
// ---------------------------------------------------

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseForwardedHeaders();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ibn Al-Zumar API v1");
    c.RoutePrefix = "swagger";
});

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// ---------------------------------------------------------------------------
// Custom Static Files Configuration
// ---------------------------------------------------------------------------
var contentTypeProvider = new FileExtensionContentTypeProvider();
contentTypeProvider.Mappings[".webp"] = "image/webp";

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = contentTypeProvider,
    ServeUnknownFileTypes = true
});

var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "uploads");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads",
    ContentTypeProvider = contentTypeProvider,
    ServeUnknownFileTypes = true
});

app.UseRouting();

app.UseCors(CorsPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();