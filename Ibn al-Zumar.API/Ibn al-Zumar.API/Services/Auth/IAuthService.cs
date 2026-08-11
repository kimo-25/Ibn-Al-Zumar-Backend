using IbnAlZumar.Api.DTOs.Auth;

namespace IbnAlZumar.Api.Services.Auth;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    Task<LoginResponseDto> GoogleLoginAsync(GoogleLoginRequestDto request); 
    Task<UpdateProfileRequestDto> GetProfileAsync(int userId);
    Task<LoginResponseDto> RegisterAsync(RegisterRequestDto request);
    Task UpdateProfileAsync(int userId, UpdateProfileRequestDto request);
}