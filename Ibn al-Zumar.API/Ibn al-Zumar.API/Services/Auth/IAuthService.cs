// File: Services/Auth/IAuthService.cs
using IbnAlZumar.Api.DTOs.Auth;

namespace IbnAlZumar.Api.Services.Auth;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
}