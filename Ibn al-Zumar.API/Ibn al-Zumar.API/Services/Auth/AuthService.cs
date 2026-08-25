using Google.Apis.Auth;
using IbnAlZumar.Api.Common.Exceptions;
using IbnAlZumar.Api.Common.Settings;
using IbnAlZumar.Api.DTOs.Auth;
using IbnAlZumar.Api.Services.Email;
using IbnAlZumar.API.Common.Exceptions;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.Domain.Entities.Identity;
using IbnAlZumar.Domain.Entities.Sales;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IbnAlZumar.Api.Services.Auth;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly JwtSettings _jwtSettings;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        ApplicationDbContext context,
        IPasswordHasher<User> passwordHasher,
        IOptions<JwtSettings> jwtOptions,
        IEmailService emailService,
        ILogger<AuthService> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtSettings = jwtOptions.Value;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var normalizedUsername = request.Username.Trim().ToLower();
        var user = await _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role).ThenInclude(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .Include(u => u.UserPermissions).ThenInclude(up => up.Permission)
            .FirstOrDefaultAsync(u => u.Username.ToLower() == normalizedUsername || (u.Email != null && u.Email.ToLower() == normalizedUsername));

        if (user is null || !user.IsActive)
        {
            throw new UnauthorizedAppException("Invalid username or password.");
        }

        var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash ?? string.Empty, request.Password);
        if (verification == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAppException("Invalid username or password.");
        }

        if (!user.IsEmailVerified)
        {
            throw new BadRequestException("Please verify your email address before logging in.");
        }

        var roleNames = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var permissionCodes = ResolveEffectivePermissions(user);

        var token = GenerateJwtToken(user, roleNames, permissionCodes, out var expiresAtUtc);

        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return new LoginResponseDto
        {
            Token = token,
            ExpiresAtUtc = expiresAtUtc,
            UserId = user.Id,
            FullName = user.FullName ?? string.Empty,
            Username = user.Username ?? string.Empty,
            Roles = roleNames,
            Permissions = permissionCodes
        };
    }

    public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        var normalizedEmail = request.Email.Trim().ToLower();
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => (u.Email != null && u.Email.ToLower() == normalizedEmail) || u.Username.ToLower() == normalizedEmail);

        if (existingUser != null)
        {
            throw new BadRequestException("User with this email already exists.");
        }

        var otp = Random.Shared.Next(100000, 999999).ToString();

        var user = new User
        {
            Username = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            IsActive = true,
            IsEmailVerified = false,
            EmailVerificationCode = otp,
            EmailVerificationExpiry = DateTime.UtcNow.AddMinutes(10),
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _context.Users.Add(user);

        var customer = new Customer
        {
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            IsRegistered = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Customers.Add(customer);

        var customerRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Customer");
        if (customerRole != null)
        {
            _context.UserRoles.Add(new UserRole
            {
                User = user,
                Role = customerRole
            });
        }

        await _context.SaveChangesAsync();

        await _emailService.SendEmailAsync(
            user.Email,
            "Verify Your Email - Ibn Al Zumar",
            $@"<h2>Welcome to Ibn Al Zumar</h2>
               <p>Your email verification code is:</p>
               <h1 style='color:#2b6cb0;'>{otp}</h1>
               <p>This code will expire in 10 minutes.</p>");

        return new RegisterResponseDto
        {
            Message = "Verification code sent to your email."
        };
    }

    public async Task VerifyEmailAsync(VerifyEmailRequestDto request)
    {
        var normalizedEmail = request.Email.Trim().ToLower();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower() == normalizedEmail);
        if (user == null)
            throw new NotFoundException("User not found.");

        if (user.IsEmailVerified)
            throw new BadRequestException("Email is already verified.");

        if (user.EmailVerificationCode != request.Code || user.EmailVerificationExpiry < DateTime.UtcNow)
        {
            throw new BadRequestException("Invalid or expired verification code.");
        }

        user.IsEmailVerified = true;
        user.EmailVerificationCode = null;
        user.EmailVerificationExpiry = null;

        await _context.SaveChangesAsync();
    }

    public async Task ResendVerificationCodeAsync(string email)
    {
        var normalizedEmail = email.Trim().ToLower();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower() == normalizedEmail);
        if (user == null)
            throw new NotFoundException("User not found.");

        if (user.IsEmailVerified)
            throw new BadRequestException("Email is already verified.");

        var otp = Random.Shared.Next(100000, 999999).ToString();
        user.EmailVerificationCode = otp;
        user.EmailVerificationExpiry = DateTime.UtcNow.AddMinutes(10);

        await _context.SaveChangesAsync();

        await _emailService.SendEmailAsync(
            user.Email!,
            "New Verification Code - Ibn Al Zumar",
            $"<p>Your new verification code is:</p><h1>{otp}</h1>");
    }

    public async Task ChangeEmailAsync(int userId, ChangeEmailRequestDto request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            throw new NotFoundException("User not found.");

        if (!string.IsNullOrEmpty(user.PasswordHash))
        {
            var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (verification == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Current password is incorrect.");
            }
        }

        var normalizedNewEmail = request.NewEmail.Trim().ToLower();
        var emailExists = await _context.Users.AnyAsync(u =>
            (u.Email != null && u.Email.ToLower() == normalizedNewEmail) ||
            (u.PendingEmail != null && u.PendingEmail.ToLower() == normalizedNewEmail));

        if (emailExists)
            throw new BadRequestException("This email is already in use.");

        var otp = Random.Shared.Next(100000, 999999).ToString();

        user.PendingEmail = request.NewEmail;
        user.PendingEmailCode = otp;
        user.PendingEmailExpiry = DateTime.UtcNow.AddMinutes(10);

        await _context.SaveChangesAsync();

        await _emailService.SendEmailAsync(
            request.NewEmail,
            "Verify Your New Email - Ibn Al Zumar",
            $"<p>Your verification code to change your email is:</p><h1>{otp}</h1><p>Expires in 10 minutes.</p>");
    }

    public async Task ResendNewEmailCodeAsync(int userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            throw new NotFoundException("User not found.");

        if (string.IsNullOrEmpty(user.PendingEmail))
            throw new BadRequestException("No pending email change found.");

        var otp = Random.Shared.Next(100000, 999999).ToString();
        user.PendingEmailCode = otp;
        user.PendingEmailExpiry = DateTime.UtcNow.AddMinutes(10);

        await _context.SaveChangesAsync();

        await _emailService.SendEmailAsync(
            user.PendingEmail,
            "Resend: Verify Your New Email - Ibn Al Zumar",
            $"<p>Your new verification code is:</p><h1>{otp}</h1>");
    }

    public async Task<LoginResponseDto> VerifyNewEmailAsync(int userId, VerifyNewEmailRequestDto request)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role).ThenInclude(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .Include(u => u.UserPermissions).ThenInclude(up => up.Permission)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new NotFoundException("User not found.");

        if (string.IsNullOrEmpty(user.PendingEmail) || !user.PendingEmail.Equals(request.NewEmail, StringComparison.OrdinalIgnoreCase))
            throw new BadRequestException("No pending email change found for this address.");

        if (user.PendingEmailCode != request.Code || user.PendingEmailExpiry < DateTime.UtcNow)
            throw new BadRequestException("Invalid or expired verification code.");

        var customer = await _context.Customers.FirstOrDefaultAsync(c => user.Email != null && c.Email != null && c.Email.ToLower() == user.Email.ToLower());
        if (customer != null)
        {
            customer.Email = user.PendingEmail;
        }

        user.Email = user.PendingEmail;
        user.Username = user.PendingEmail;

        user.PendingEmail = null;
        user.PendingEmailCode = null;
        user.PendingEmailExpiry = null;
        user.IsEmailVerified = true;

        await _context.SaveChangesAsync();

        var roleNames = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var permissionCodes = ResolveEffectivePermissions(user);
        var token = GenerateJwtToken(user, roleNames, permissionCodes, out var expiresAtUtc);

        return new LoginResponseDto
        {
            Token = token,
            ExpiresAtUtc = expiresAtUtc,
            UserId = user.Id,
            FullName = user.FullName ?? string.Empty,
            Username = user.Username ?? string.Empty,
            Roles = roleNames,
            Permissions = permissionCodes
        };
    }

    public async Task SendPhoneOtpAsync(int userId, string phone)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) throw new NotFoundException("User not found.");

        var otp = Random.Shared.Next(100000, 999999).ToString();

        user.PendingPhone = phone;
        user.PendingPhoneCode = otp;
        user.PendingPhoneExpiry = DateTime.UtcNow.AddMinutes(10);

        await _context.SaveChangesAsync();

        _logger.LogInformation($"[SMS/WhatsApp SERVICE] OTP for User {userId} ({phone}) is: {otp}");
    }

    public async Task VerifyPhoneAsync(int userId, string code)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) throw new NotFoundException("User not found.");

        if (string.IsNullOrEmpty(user.PendingPhone))
        {
            throw new BadRequestException("No pending phone verification requested.");
        }

        if (user.PendingPhoneCode != code || user.PendingPhoneExpiry < DateTime.UtcNow)
        {
            throw new BadRequestException("Invalid or expired verification code.");
        }

        var customer = await _context.Customers.FirstOrDefaultAsync(c => user.Email != null && c.Email != null && c.Email.ToLower() == user.Email.ToLower());
        if (customer != null)
        {
            customer.Phone = user.PendingPhone;
        }

        user.IsPhoneVerified = true;
        user.PendingPhone = null;
        user.PendingPhoneCode = null;
        user.PendingPhoneExpiry = null;

        await _context.SaveChangesAsync();
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequestDto request)
    {
        var normalizedEmail = request.Email.Trim().ToLower();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower() == normalizedEmail);
        if (user == null) return;

        var otp = Random.Shared.Next(100000, 999999).ToString();
        user.PasswordResetCode = otp;
        user.PasswordResetExpiry = DateTime.UtcNow.AddMinutes(10);

        await _context.SaveChangesAsync();

        await _emailService.SendEmailAsync(
            user.Email!,
            "Password Reset Code - Ibn Al Zumar",
            $"<p>Your password reset code is:</p><h1>{otp}</h1><p>Expires in 10 minutes.</p>");
    }

    public async Task VerifyResetCodeAsync(VerifyResetCodeRequestDto request)
    {
        var normalizedEmail = request.Email.Trim().ToLower();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower() == normalizedEmail);
        if (user == null || user.PasswordResetCode != request.Code || user.PasswordResetExpiry < DateTime.UtcNow)
        {
            throw new BadRequestException("Invalid or expired reset code.");
        }
    }

    public async Task ResetPasswordAsync(ResetPasswordRequestDto request)
    {
        var normalizedEmail = request.Email.Trim().ToLower();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower() == normalizedEmail);
        if (user == null || user.PasswordResetCode != request.Code || user.PasswordResetExpiry < DateTime.UtcNow)
        {
            throw new BadRequestException("Invalid or expired reset code.");
        }

        user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
        user.PasswordResetCode = null;
        user.PasswordResetExpiry = null;

        await _context.SaveChangesAsync();
    }

    public async Task ChangePasswordAsync(int userId, ChangePasswordRequestDto request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            throw new UnauthorizedAppException("User not found.");

        if (string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            throw new BadRequestException("This account was created via Google Sign-In and does not have a local password.");
        }

        if (request.NewPassword != request.ConfirmPassword)
            throw new BadRequestException("Passwords do not match.");

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);

        if (result == PasswordVerificationResult.Failed)
            throw new BadRequestException("Current password is incorrect.");

        user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);

        await _context.SaveChangesAsync();
    }

    public async Task<LoginResponseDto> GoogleLoginAsync(GoogleLoginRequestDto request)
    {
        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);
        }
        catch (Exception)
        {
            throw new UnauthorizedAppException("Invalid Google token.");
        }

        var normalizedEmail = payload.Email.Trim().ToLower();
        var user = await _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role).ThenInclude(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .Include(u => u.UserPermissions).ThenInclude(up => up.Permission)
            .FirstOrDefaultAsync(u => u.Username.ToLower() == normalizedEmail || (u.Email != null && u.Email.ToLower() == normalizedEmail));

        if (user is null)
        {
            user = new User
            {
                Username = payload.Email,
                Email = payload.Email,
                FullName = payload.Name ?? "Google User",
                PasswordHash = string.Empty,
                IsActive = true,
                IsEmailVerified = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);

            var customer = new Customer
            {
                FullName = payload.Name ?? "Google User",
                Email = payload.Email,
                IsRegistered = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Customers.Add(customer);

            var customerRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Customer");
            if (customerRole != null)
            {
                _context.UserRoles.Add(new UserRole
                {
                    User = user,
                    Role = customerRole
                });
            }

            await _context.SaveChangesAsync();
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAppException("This account has been deactivated.");
        }

        var roleNames = user.UserRoles?.Select(ur => ur.Role.Name).ToList() ?? new List<string>();
        var permissionCodes = ResolveEffectivePermissions(user);

        var token = GenerateJwtToken(user, roleNames, permissionCodes, out var expiresAtUtc);

        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return new LoginResponseDto
        {
            Token = token,
            ExpiresAtUtc = expiresAtUtc,
            UserId = user.Id,
            FullName = user.FullName ?? string.Empty,
            Username = user.Username ?? string.Empty,
            Roles = roleNames,
            Permissions = permissionCodes
        };
    }

    public async Task<UserProfileDto> GetProfileAsync(int userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            throw new NotFoundException("User not found.");
        }

        var customer = await _context.Customers.FirstOrDefaultAsync(c => user.Email != null && c.Email != null && c.Email.ToLower() == user.Email.ToLower());

        return new UserProfileDto
        {
            FullName = user.FullName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Phone = customer?.Phone ?? string.Empty,
            Governorate = customer?.Governorate ?? string.Empty,
            Address = customer?.Address ?? string.Empty,
            IsEmailVerified = user.IsEmailVerified,
            IsPhoneVerified = user.IsPhoneVerified,
            HasPassword = user.HasPassword
        };
    }

    public async Task UpdateProfileAsync(int userId, UpdateProfileRequestDto request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            throw new UnauthorizedAppException("User not found.");
        }

        user.FullName = request.FullName;

        var customer = await _context.Customers.FirstOrDefaultAsync(c => user.Email != null && c.Email != null && c.Email.ToLower() == user.Email.ToLower());
        if (customer != null)
        {
            customer.FullName = request.FullName;
            customer.Governorate = request.Governorate;
            customer.Address = request.Address;
        }

        await _context.SaveChangesAsync();
    }

    private static List<string> ResolveEffectivePermissions(User user)
    {
        var fromRoles = user.UserRoles.SelectMany(ur => ur.Role.RolePermissions).Select(rp => rp.Permission.Code);
        var effective = new HashSet<string>(fromRoles, StringComparer.OrdinalIgnoreCase);

        foreach (var overrideEntry in user.UserPermissions)
        {
            if (overrideEntry.IsGranted)
                effective.Add(overrideEntry.Permission.Code);
            else
                effective.Remove(overrideEntry.Permission.Code);
        }

        return effective.ToList();
    }

    private string GenerateJwtToken(User user, List<string> roles, List<string> permissions, out DateTime expiresAtUtc)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new("fullName", user.FullName ?? string.Empty),
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
        claims.AddRange(permissions.Select(p => new Claim("permission", p)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        expiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}