using System.ComponentModel.DataAnnotations;

namespace IbnAlZumar.Api.DTOs.Auth;

public class RegisterRequestDto
{
    [Required, MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
    public string Password { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;
}