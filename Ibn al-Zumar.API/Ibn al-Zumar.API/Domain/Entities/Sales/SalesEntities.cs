using System.ComponentModel.DataAnnotations;
using IbnAlZumar.Domain.Common;
using IbnAlZumar.Domain.Entities.Catalog;
using IbnAlZumar.Domain.Entities.Identity;
using IbnAlZumar.Domain.Entities.Inventory;
using IbnAlZumar.Domain.Enums;

namespace IbnAlZumar.Domain.Entities.Sales;

/// <summary>
/// A "real" customer record — used for registered online accounts AND for walk-in customers
/// the cashier chooses to save (e.g. for warranty tracking or to run a debt tab).
/// True anonymous, one-off checkouts don't need a Customer row at all: see Order.GuestName/GuestPhone.
/// </summary>
public class Customer : BaseEntity
{
    [Required, MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(30)]
    public string? Phone { get; set; }

    [MaxLength(150)]
    public string? Email { get; set; }

    [MaxLength(300)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? Governorate { get; set; }

    /// <summary>True for accounts created via online registration/login; false for quick walk-in records.</summary>
    public bool IsRegistered { get; set; } = true;

    public decimal CreditLimit { get; set; } = 0;

    /// <summary>Positive = customer owes the store ("الشكك"). Kept in sync via CustomerLedgerEntry rows.</summary>
    public decimal CurrentBalance { get; set; } = 0;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<CustomerLedgerEntry> LedgerEntries { get; set; } = new List<CustomerLedgerEntry>();
}

/// <summary>
/// Works for both an online COD order and an in-store POS sale — Source/PaymentMethod/CashierUserId
/// distinguish the two, so no separate "OnlineOrder"/"POSSale" tables are needed.
/// </summary>
public class Order : BaseEntity
{
    [Required, MaxLength(50)]
    public string OrderNumber { get; set; } = string.Empty;

    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    /// <summary>Used when there is no Customer row at all (anonymous online COD or quick POS sale).</summary>
    [MaxLength(150)]
    public string? GuestName { get; set; }

    [MaxLength(30)]
    public string? GuestPhone { get; set; }

    public OrderSource Source { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.PendingConfirmation;
    public PaymentMethod PaymentMethod { get; set; }

    /// <summary>Fulfilling warehouse. Defaults to Id = 1 in Phase 1; picked explicitly in Phase 2 POS.</summary>
    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;

    /// <summary>Null for online orders in Phase 1; set to the logged-in cashier for Phase 2 POS sales.</summary>
    public int? CashierUserId { get; set; }
    public User? CashierUser { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [MaxLength(300)]
    public string? ShippingAddress { get; set; }

    [MaxLength(100)]
    public string? DeliveryGovernorate { get; set; }

    public decimal SubTotal { get; set; }

    public DiscountType DiscountType { get; set; } = DiscountType.None;
    public decimal DiscountValue { get; set; } // percentage or fixed amount, per DiscountType
    public decimal DiscountAmount { get; set; } // computed, always in currency

    public decimal TotalAmount { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

public class OrderItem : BaseEntity
{
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public DiscountType DiscountType { get; set; } = DiscountType.None;
    public decimal DiscountValue { get; set; }
    public decimal DiscountAmount { get; set; }

    public decimal LineTotal { get; set; }
}

/// <summary>
/// A cash/card/InstaPay movement — can settle an Order in full at checkout, OR be a later,
/// standalone debt collection against a Customer (OrderId null, CustomerId set).
/// </summary>
public class Payment : BaseEntity
{
    public int? OrderId { get; set; }
    public Order? Order { get; set; }

    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }

    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    public int? ReceivedByUserId { get; set; }
    public User? ReceivedByUser { get; set; }

    [MaxLength(300)]
    public string? Notes { get; set; }
}

/// <summary>
/// Append-only debt ledger ("الشكك"). Every SaleOnCredit / PaymentReceived / ManualAdjustment
/// writes one row here; Customer.CurrentBalance is the running total, RunningBalance is the
/// snapshot after this specific entry (useful for printing a statement).
/// </summary>
public class CustomerLedgerEntry : BaseEntity
{
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public LedgerTransactionType TransactionType { get; set; }

    /// <summary>Always positive; sign/effect is implied by TransactionType.</summary>
    public decimal Amount { get; set; }

    public decimal RunningBalance { get; set; }

    public int? RelatedOrderId { get; set; }
    public Order? RelatedOrder { get; set; }

    public int? RelatedPaymentId { get; set; }
    public Payment? RelatedPayment { get; set; }

    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    [MaxLength(300)]
    public string? Notes { get; set; }
}
