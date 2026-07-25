using IbnAlZumar.API.Common.Exceptions;
using IbnAlZumar.API.DTOs.Catalog;
using IbnAlZumar.API.DTOs.Identity;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IbnAlZumar.API.Services.Identity;

public class UserManagementService : IUserManagementService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserManagementService(ApplicationDbContext context, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<PagedResultDto<UserDto>> GetUsersAsync(int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.Users
            .AsNoTracking()
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .AsQueryable();

        var totalCount = await query.CountAsync();
        var users = await query
            .OrderBy(u => u.Username)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email ?? string.Empty,
                FullName = u.FullName ?? string.Empty,
                IsActive = u.IsActive,
                Roles = u.UserRoles.Select(ur => ur.Role.Name).Where(r => r != null).ToList()!
            })
            .ToListAsync();

        return new PagedResultDto<UserDto>
        {
            Items = users,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<UserDto> GetUserByIdAsync(int id)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) throw new NotFoundException($"المستخدم رقم {id} غير موجود");

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName ?? string.Empty,
            IsActive = user.IsActive,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).Where(r => r != null).ToList()!
        };
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
    {
        var exists = await _context.Users.AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email);
        if (exists) throw new BadRequestException("اسم المستخدم أو البريد الإلكتروني مستخدم بالفعل");

        var user = new User
        {
            Username = dto.Username.Trim(),
            Email = dto.Email.Trim().ToLower(),
            FullName = dto.FullName.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        foreach (var roleId in dto.RoleIds)
        {
            user.UserRoles.Add(new UserRole { RoleId = roleId });
        }

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return await GetUserByIdAsync(user.Id);
    }

    public async Task UpdateUserRolesAsync(int userId, UpdateUserRolesDto dto)
    {
        var user = await _context.Users.Include(u => u.UserRoles).FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) throw new NotFoundException($"المستخدم رقم {userId} غير موجود");

        user.UserRoles.Clear();
        foreach (var roleId in dto.RoleIds)
        {
            user.UserRoles.Add(new UserRole { UserId = userId, RoleId = roleId });
        }

        await _context.SaveChangesAsync();
    }

    public async Task ToggleUserStatusAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new NotFoundException($"المستخدم رقم {userId} غير موجود");

        user.IsActive = !user.IsActive;
        await _context.SaveChangesAsync();
    }

    public async Task<List<RoleDto>> GetRolesAsync()
    {
        return await _context.Roles
            .AsNoTracking()
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Permissions = r.RolePermissions.Select(rp => rp.Permission.Name).Where(p => p != null).ToList()!
            })
            .ToListAsync();
    }

    public async Task<RoleDto> CreateRoleAsync(CreateRoleDto dto)
    {
        var role = new Role
        {
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim()
        };

        var permissions = await _context.Permissions
            .Where(p => dto.Permissions.Contains(p.Name))
            .ToListAsync();

        foreach (var perm in permissions)
        {
            role.RolePermissions.Add(new RolePermission { PermissionId = perm.Id });
        }

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            Permissions = permissions.Select(p => p.Name).ToList()
        };
    }

    public async Task<List<string>> GetAllAvailablePermissionsAsync()
    {
        return await _context.Permissions.Select(p => p.Name).ToListAsync();
    }
}