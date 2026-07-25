// File: DTOs/Auth/LoginResponseDto.cs
namespace IbnAlZumar.Api.DTOs.Auth;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }

    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;

    public List<string> Roles { get; set; } = new();

    /// <summary>Effective permission codes (role defaults + per-user overrides applied).
    /// Handy for the frontend to show/hide UI without waiting on a 403.</summary>
    public List<string> Permissions { get; set; } = new();
}