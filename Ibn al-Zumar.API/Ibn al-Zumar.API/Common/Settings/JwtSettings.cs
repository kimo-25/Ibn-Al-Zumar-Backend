// File: Common/Settings/JwtSettings.cs
namespace IbnAlZumar.Api.Common.Settings;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;

    /// <summary>Symmetric signing key. Must be at least 32 characters (256 bits) for HS256.
    /// In production, override this via environment variable or user-secrets — never commit a real key.</summary>
    public string Key { get; set; } = string.Empty;

    public int ExpiryMinutes { get; set; } = 120;
}