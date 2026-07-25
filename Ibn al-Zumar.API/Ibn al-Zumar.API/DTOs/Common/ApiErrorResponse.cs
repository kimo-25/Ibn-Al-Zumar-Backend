// File: DTOs/Common/ApiErrorResponse.cs
namespace IbnAlZumar.Api.DTOs.Common;

public class ApiErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? TraceId { get; set; }

    /// <summary>Field-level validation errors, populated only for validation failures.</summary>
    public IDictionary<string, string[]>? Errors { get; set; }

    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
}