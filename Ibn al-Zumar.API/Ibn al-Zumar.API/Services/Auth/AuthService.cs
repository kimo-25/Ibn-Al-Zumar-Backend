// File: Services/Auth/AuthService.cs
using IbnAlZumar.Api.Common.Exceptions;
using IbnAlZumar.Api.Common.Settings;
using IbnAlZumar.Api.DTOs.Auth;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.Domain.Entities.Identity;
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

        // Same error for "no such user" and "wrong password" — don't reveal which usernames exist.
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

    /// <summary>
    /// Effective permission = union of all the user's role defaults (via UserRole -> RolePermission),
    /// then per-user UserPermission overrides applied on top: IsGranted = true force-adds a
    /// permission even if no role grants it; IsGranted = false force-removes one even if a role does.
    /// This is the same resolution rule described on the UserPermission entity itself.
    /// </summary>
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
            new(ClaimTypes.Name, user.Username),
            new("fullName", user.FullName),
        };

        // ClaimTypes.Role claims light up [Authorize(Roles = "Admin")] for free.
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        // Custom "permission" claims are what the PermissionAuthorizationHandler checks against.
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