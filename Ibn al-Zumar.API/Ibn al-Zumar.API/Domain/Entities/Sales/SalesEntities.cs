using System.ComponentModel.DataAnnotations;
using IbnAlZumar.Domain.Common;
using IbnAlZumar.Domain.Entities.Catalog;
using IbnAlZumar.Domain.Entities.Identity;
using IbnAlZumar.Domain.Entities.Inventory;
using IbnAlZumar.Domain.Enums;

namespace IbnAlZumar.Domain.Entities.Sales;

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

    public bool IsRegistered { get; set; } = true;

    public decimal CreditLimit { get; set; } = 0;

    public decimal CurrentBalance { get; set; } = 0;

    // 👈 إضافة الشريحة الافتراضية للعميل
    public PricingTierType DefaultPricingTier { get; set; } = PricingTierType.Retail;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<CustomerLedgerEntry> LedgerEntries { get; set; } = new List<CustomerLedgerEntry>();
}

public class Order : BaseEntity
{
    [MaxLength(64)]
    public string? ClientUuid { get; set; }

    [Required, MaxLength(50)]
    public string OrderNumber { get; set; } = string.Empty;

    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    [MaxLength(150)]
    public string? GuestName { get; set; }

    [MaxLength(30)]
    public string? GuestPhone { get; set; }

    public OrderSource Source { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.PendingConfirmation;
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.CodPending;

    // 👈 إضافة شريحة التسعير المطبقة على الفاتورة
    public PricingTierType PricingTier { get; set; } = PricingTierType.Retail;

    [MaxLength(100)]
    public string? PaymobOrderId { get; set; }

    [MaxLength(100)]
    public string? PaymobTransactionId { get; set; }

    public int WarehouseId { get; set; }

    [Required]
    public Warehouse Warehouse { get; set; } = null!;

    public int? CashierUserId { get; set; }
    public User? CashierUser { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [MaxLength(300)]
    public string? ShippingAddress { get; set; }

    [MaxLength(100)]
    public string? DeliveryGovernorate { get; set; }
    public int? ShippingZoneId { get; set; }

    public ShippingZone? ShippingZone { get; set; }

    public bool IsCustomZoneRequested { get; set; } = false;

    [MaxLength(150)]
    public string? CustomZoneName { get; set; }

    public CustomZoneRequestStatus CustomZoneRequestStatus { get; set; } = CustomZoneRequestStatus.None;

    public decimal SubTotal { get; set; }

    public DiscountType DiscountType { get; set; } = DiscountType.None;
    public decimal DiscountValue { get; set; }
    public decimal DiscountAmount { get; set; }

    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }

    public decimal TotalAmount { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    [MaxLength(500)]
    public string? CancellationReason { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

public class OrderItem : BaseEntity
{
    public int OrderId { get; set; }
    [Required]
    public Order Order { get; set; } = null!;

    public int ProductId { get; set; }
    [Required]
    public Product Product { get; set; } = null!;

    public int? ProductVariantId { get; set; }

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    // New: store the unit cost price (for profit / cost analysis)
    public decimal UnitCostPrice { get; set; }

    public DiscountType DiscountType { get; set; } = DiscountType.None;
    public decimal DiscountValue { get; set; }
    public decimal DiscountAmount { get; set; }

    public decimal LineTotal { get; set; }
}

public class Payment : BaseEntity
{
    public int? OrderId { get; set; }
    public Order? Order { get; set; }

    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    [MaxLength(100)]
    public string? PaymobTransactionId { get; set; }

    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    public int? ReceivedByUserId { get; set; }
    public User? ReceivedByUser { get; set; }

    [MaxLength(300)]
    public string? Notes { get; set; }
}

public class CustomerLedgerEntry : BaseEntity
{
    public int CustomerId { get; set; }
    [Required]
    public Customer Customer { get; set; } = null!;

    public LedgerTransactionType TransactionType { get; set; }

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