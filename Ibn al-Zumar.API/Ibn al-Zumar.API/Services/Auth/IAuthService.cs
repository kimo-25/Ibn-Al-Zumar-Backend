using IbnAlZumar.Api.DTOs.Auth;

namespace IbnAlZumar.Api.Services.Auth;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    Task<LoginResponseDto> GoogleLoginAsync(GoogleLoginRequestDto request);
    Task<UserProfileDto> GetProfileAsync(int userId);
    Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request);
    Task UpdateProfileAsync(int userId, UpdateProfileRequestDto request);
    Task ChangePasswordAsync(int userId, ChangePasswordRequestDto request);

    // تغيير البريد وإعادة إرسال الكود
    Task ChangeEmailAsync(int userId, ChangeEmailRequestDto request);
    Task<LoginResponseDto> VerifyNewEmailAsync(int userId, VerifyNewEmailRequestDto request);
    Task ResendNewEmailCodeAsync(int userId);

    // التحقق من الهاتف وتغييره معزولاً
    Task SendPhoneOtpAsync(int userId, string phone);
    Task VerifyPhoneAsync(int userId, string code);

    // التحقق العام وإعادة التعيين
    Task VerifyEmailAsync(VerifyEmailRequestDto request);
    Task ResendVerificationCodeAsync(string email);
    Task ForgotPasswordAsync(ForgotPasswordRequestDto request);
    Task VerifyResetCodeAsync(VerifyResetCodeRequestDto request);
    Task ResetPasswordAsync(ResetPasswordRequestDto request);
}