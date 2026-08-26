namespace IbnAlZumar.API.Services.Ai;

public sealed record AiAuditEntry(
    int? UserId,
    string? UserEmail,
    IReadOnlyCollection<string> Roles,
    string Action,
    string? Prompt = null,
    string? ToolName = null,
    bool Succeeded = true,
    string? Error = null,
    string? MetadataJson = null,
    string? IpAddress = null);

public interface IAiAuditLogService
{
    Task LogAsync(AiAuditEntry entry, CancellationToken cancellationToken = default);
}