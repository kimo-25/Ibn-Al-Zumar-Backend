using System.ComponentModel.DataAnnotations;
using IbnAlZumar.Domain.Enums;

namespace IbnAlZumar.API.DTOs.Sync;

/// <summary>
/// One offline-created order, as stored in the client's Dexie queue.
/// Mirrors the shape of CreateOrderDto but carries a ClientUuid and
/// is intentionally flat (no server-computed fields trusted from the client).
/// </summary>
public class SyncOrderDto
{
    [Required, MaxLength(64)]
    public string ClientUuid { get; set; } = string.Empty;

    public int? CustomerId { get; set; }

    [MaxLength(150)]
    public string? GuestName { get; set; }

    [MaxLength(30)]
    public string? GuestPhone { get; set; }

    [Required]
    public OrderSource Source { get; set; }

    [Required]
    public PaymentMethod PaymentMethod { get; set; }

    [Required]
    public int WarehouseId { get; set; }

    /// <summary>Client-recorded timestamp of when the sale actually happened offline.</summary>
    [Required]
    public DateTime OrderDate { get; set; }

    [MaxLength(300)]
    public string? ShippingAddress { get; set; }

    public DiscountType DiscountType { get; set; } = DiscountType.None;
    public decimal DiscountValue { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    [Required, MinLength(1, ErrorMessage = "Order must contain at least one item.")]
    public List<SyncOrderItemDto> Items { get; set; } = new();
}

public class SyncOrderItemDto
{
    [Required]
    public int ProductId { get; set; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "Quantity must be positive.")]
    public int Quantity { get; set; }

    [Required, Range(0, double.MaxValue, ErrorMessage = "UnitPrice cannot be negative.")]
    public decimal UnitPrice { get; set; }

    public DiscountType DiscountType { get; set; } = DiscountType.None;
    public decimal DiscountValue { get; set; }
}

/// <summary>Top-level request body for POST /api/orders/sync.</summary>
public class SyncBatchRequestDto
{
    [Required, MinLength(1), MaxLength(200, ErrorMessage = "Batch too large; split into smaller syncs.")]
    public List<SyncOrderDto> Orders { get; set; } = new();
}

public class SyncResultDto
{
    public string ClientUuid { get; set; } = string.Empty;
    public bool Success { get; set; }

    /// <summary>Server-assigned Id when Success is true, so the client can store it if needed.</summary>
    public int? ServerId { get; set; }

    /// <summary>Machine-readable reason, useful for client-side retry logic (e.g. skip permanent failures).</summary>
    public string? ErrorCode { get; set; }

    public string? ErrorMessage { get; set; }
}

public class SyncBatchResponseDto
{
    public int TotalReceived { get; set; }
    public int SucceededCount { get; set; }
    public int FailedCount { get; set; }
    public List<SyncResultDto> Results { get; set; } = new();
}