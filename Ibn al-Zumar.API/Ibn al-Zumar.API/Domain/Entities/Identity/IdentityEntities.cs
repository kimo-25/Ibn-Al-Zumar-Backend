using System.ComponentModel.DataAnnotations;
using IbnAlZumar.Domain.Common;
using IbnAlZumar.Domain.Entities.Attendance;

namespace IbnAlZumar.Domain.Entities.Identity;

public class User : BaseEntity
{
    [Required, MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? Email { get; set; }

    [MaxLength(300)]
    public string PasswordHash { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime? LastLoginAt { get; set; }

    // --- Œ«’… » ›⁄Ì· «·»—Ìœ «·≈·ﬂ —Ê‰Ì ⁄‰œ ≈‰‘«¡ «·Õ”«» ---
    public bool IsEmailVerified { get; set; } = false;

    [MaxLength(10)]
    public string? EmailVerificationCode { get; set; }

    public DateTime? EmailVerificationExpiry { get; set; }

    [MaxLength(10)]
    public string? PasswordResetCode { get; set; }

    public DateTime? PasswordResetExpiry { get; set; }

    // ---  €ÌÌ— «·»—Ìœ «·≈·ﬂ —Ê‰Ì ⁄»— OTP ---
    [MaxLength(150)]
    public string? PendingEmail { get; set; }

    [MaxLength(10)]
    public string? PendingEmailCode { get; set; }

    public DateTime? PendingEmailExpiry { get; set; }

    // --- Œ«’… »—ﬁ„ «·Â« › Ê«· Õﬁﬁ ⁄»— OTP ( ›⁄Ì· ·«Õﬁ «Œ Ì«—Ì) ---
    public bool IsPhoneVerified { get; set; } = false;

    [MaxLength(20)]
    public string? PendingPhone { get; set; }

    [MaxLength(10)]
    public string? PendingPhoneCode { get; set; }

    public DateTime? PendingPhoneExpiry { get; set; }

    // --- Œ«’… » ”ÃÌ· «·œŒÊ· ⁄»— „“ÊœÌ‰ Œ«—ÃÌÌ‰ „À· Google ---
    public bool HasPassword => !string.IsNullOrEmpty(PasswordHash);

    // --- «·Õ÷Ê— Ê«·«‰’—«› Ê«·—Ê« » (Voice Biometric Attendance) ---
    [Range(0, double.MaxValue, ErrorMessage = "√Ã— «·”«⁄… ÌÃ» √‰ ÌﬂÊ‰ —ﬁ„« „ÊÃ»«.")]
    public decimal HourlyRate { get; set; }

    /// <summary>
    /// «·»’„… «·’Ê Ì… ··„ÊŸ› (Voice Embedding) „Œ“‰… ﬂ‹ JSON Array „‰ «·√—ﬁ«„
    /// ﬂ„« Ì „ «” —Ã«⁄Â« „‰ Hugging Face Inference API ·„ÊœÌ· speechbrain/spkrec-ecapa-voxceleb.
    /// </summary>
    public string? VoiceEmbedding { get; set; }

    public ICollection<AttendanceLog> AttendanceLogs { get; set; } = new List<AttendanceLog>();
    public ICollection<PayrollRecord> PayrollRecords { get; set; } = new List<PayrollRecord>();

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}

public class Role : BaseEntity
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Description { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

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

public class UserRole
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
}

public class RolePermission
{
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public int PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
}

public class UserPermission
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;

    public bool IsGranted { get; set; }
}
