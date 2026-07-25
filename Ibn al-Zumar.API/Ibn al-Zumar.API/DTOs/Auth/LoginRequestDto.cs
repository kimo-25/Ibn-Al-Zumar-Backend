// File: DTOs/Auth/LoginRequestDto.cs
using System.ComponentModel.DataAnnotations;

namespace IbnAlZumar.Api.DTOs.Auth;

public class LoginRequestDto
{
    [Required, MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Password { get; set; } = string.Empty;
}