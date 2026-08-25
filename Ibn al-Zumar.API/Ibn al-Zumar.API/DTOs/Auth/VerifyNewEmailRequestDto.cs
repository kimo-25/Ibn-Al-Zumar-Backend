namespace IbnAlZumar.Api.DTOs.Auth;

public class VerifyNewEmailRequestDto
{
    public string NewEmail { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}