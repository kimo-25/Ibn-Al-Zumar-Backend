namespace IbnAlZumar.Domain.Common;

/// <summary>
/// Base class for all entities. Provides Id, audit timestamps, and a soft-delete flag.
/// A global query filter (IsDeleted == false) is applied to every derived entity
/// in ApplicationDbContext.OnModelCreating, so records are never hard-deleted by default.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; } = false;
}
