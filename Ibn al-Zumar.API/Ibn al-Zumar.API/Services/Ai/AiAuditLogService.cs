using System.Text.Json;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.Domain.Entities.Ai;
namespace IbnAlZumar.API.Services.Ai;
public sealed class AiAuditLogService : IAiAuditLogService
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<AiAuditLogService> _logger;
    public AiAuditLogService(ApplicationDbContext db, ILogger<AiAuditLogService> logger)
    {
        _db = db;
        _logger = logger;
    }
    public async Task LogAsync(AiAuditEntry entry, CancellationToken cancellationToken = default)
    {
        try
        {
            _db.AiAuditLogs.Add(new AiAuditLog
            {
                UserId = entry.UserId,
                UserEmail = Limit(entry.UserEmail, 320),
                Roles = Limit(string.Join(",", entry.Roles ?? Array.Empty<string>()), 1000) ?? string.Empty,
                Action = Limit(entry.Action, 64) ?? "query",
                Prompt = Limit(entry.Prompt, 12000),
                ToolName = Limit(entry.ToolName, 128),
                Succeeded = entry.Succeeded,
                Error = Limit(entry.Error, 2000),
                MetadataJson = Limit(entry.MetadataJson, 12000),
                IpAddress = Limit(entry.IpAddress, 64),
                TimestampUtc = DateTime.UtcNow
            });
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            // Auditing must never make a customer-facing AI request fail, but the loss is observable.
            _logger.LogError(ex, "Failed to persist AI audit event {Action} {ToolName}", entry.Action, entry.ToolName);
        }
    }
    private static string? Limit(string? value, int max) =>
        string.IsNullOrEmpty(value) ? value : value.Length <= max ? value : value[..max];
}