namespace IbnAlZumar.Domain.Entities.Ai;

public sealed class AiAuditLog
{
    public long Id { get; set; }
    public int? UserId { get; set; }
    public string? UserEmail { get; set; }
    public string Roles { get; set; } = string.Empty;
    public string Action { get; set; } = "query";
    public string? Prompt { get; set; }
    public string? ToolName { get; set; }
    public bool Succeeded { get; set; }
    public string? Error { get; set; }
    public string? MetadataJson { get; set; }
    public string? IpAddress { get; set; }
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
}