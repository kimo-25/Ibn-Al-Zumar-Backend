using IbnAlZumar.Api.DTOs.Auth;

namespace IbnAlZumar.Api.Services.Auth;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    Task<LoginResponseDto> GoogleLoginAsync(GoogleLoginRequestDto request); // الدالة الجديدة

    Task<LoginResponseDto> RegisterAsync(RegisterRequestDto request);
    Task UpdateProfileAsync(int userId, UpdateProfileRequestDto request);
}