using Google.Apis.Auth;
using IbnAlZumar.Api.Common.Exceptions;
using IbnAlZumar.Api.Common.Settings;
using IbnAlZumar.Api.DTOs.Auth;
using IbnAlZumar.API.Common.Exceptions;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.Domain.Entities.Identity;
using IbnAlZumar.Domain.Entities.Sales;
using IbnAlZumar.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

    public AuthService(ApplicationDbContext context, IPasswordHasher<User> passwordHasher, IOptions<JwtSettings> jwtOptions)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role).ThenInclude(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .Include(u => u.UserPermissions).ThenInclude(up => up.Permission)
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user is null || !user.IsActive)
        {
            throw new UnauthorizedAppException("Invalid username or password.");
        }

        var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verification == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAppException("Invalid username or password.");
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
            FullName = user.FullName,
            Username = user.Username,
            Roles = roleNames,
            Permissions = permissionCodes
        };
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

        var user = await _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role).ThenInclude(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .Include(u => u.UserPermissions).ThenInclude(up => up.Permission)
            .FirstOrDefaultAsync(u => u.Username == payload.Email || u.Email == payload.Email);

        if (user is null)
        {
            user = new User
            {
                Username = payload.Email,
                Email = payload.Email,
                FullName = payload.Name ?? "Google User",
                PasswordHash = string.Empty,
                IsActive = true,
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
            FullName = user.FullName,
            Username = user.Username,
            Roles = roleNames,
            Permissions = permissionCodes
        };
    }

    public async Task<LoginResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email || u.Username == request.Email);

        if (existingUser != null)
        {
            throw new UnauthorizedAppException("User with this email already exists.");
        }

        var user = new User
        {
            Username = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _context.Users.Add(user);

        var customer = new Customer
        {
            FullName = request.FullName,
            Email = request.Email,
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

        var roleNames = customerRole != null ? new List<string> { customerRole.Name } : new List<string>();
        var permissionCodes = new List<string>();

        var token = GenerateJwtToken(user, roleNames, permissionCodes, out var expiresAtUtc);

        return new LoginResponseDto
        {
            Token = token,
            ExpiresAtUtc = expiresAtUtc,
            UserId = user.Id,
            FullName = user.FullName,
            Username = user.Username,
            Roles = roleNames,
            Permissions = permissionCodes
        };
    }

    public async Task<UpdateProfileRequestDto> GetProfileAsync(int userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            throw new NotFoundException("User not found.");
        }

        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == user.Email);

        return new UpdateProfileRequestDto
        {
            FullName = user.FullName,
            Phone = customer?.Phone ?? string.Empty
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

        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == user.Email);
        if (customer != null)
        {
            customer.FullName = request.FullName;
            customer.Phone = request.Phone;
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