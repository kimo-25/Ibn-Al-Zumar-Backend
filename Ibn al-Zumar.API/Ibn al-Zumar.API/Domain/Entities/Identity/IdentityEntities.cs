using System.ComponentModel.DataAnnotations;
using IbnAlZumar.Domain.Common;

namespace IbnAlZumar.Domain.Entities.Identity;

public class User : BaseEntity
{
    [Required, MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? Email { get; set; }

    [Required, MaxLength(300)]
    public string PasswordHash { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime? LastLoginAt { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}

/// <summary>e.g. "Admin", "Cashier", "OnlineManager". Carries a default permission set via RolePermission.</summary>
public class Role : BaseEntity
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Description { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

/// <summary>A single fine-grained action, e.g. Code = "CanEditProducts", Module = "Products".</summary>
public class Permission : BaseEntity
{
    [Required, MaxLength(100)]
    public string Code { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Module { get; set; }

    [MaxLength(300)]
    public string? Description { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}

/// <summary>Many-to-many join: which roles a user has.</summary>
public class UserRole
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
}

/// <summary>Many-to-many join: the *default* permission set granted by a role.</summary>
public class RolePermission
{
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public int PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
}

/// <summary>
/// Per-user override on top of role defaults. This is what powers the Admin's toggle switch:
/// a row with IsGranted = true force-grants a permission the user's role doesn't include; a row
/// with IsGranted = false force-revokes a permission the role otherwise grants. No row = fall
/// back to whatever the user's role(s) say via RolePermission. Effective-permission check is
/// application logic, not something the schema enforces.
/// </summary>
public class UserPermission
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;

    public bool IsGranted { get; set; }
}
