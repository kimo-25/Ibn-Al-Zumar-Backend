using System.ComponentModel.DataAnnotations;
using IbnAlZumar.Domain.Common;
using IbnAlZumar.Domain.Entities.Catalog;
using IbnAlZumar.Domain.Entities.Identity;
using IbnAlZumar.Domain.Entities.Inventory;
using IbnAlZumar.Domain.Enums;

namespace IbnAlZumar.Domain.Entities.Purchasing;

public class Supplier : BaseEntity
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? ContactPerson { get; set; }

    [MaxLength(30)]
    public string? Phone { get; set; }

    [MaxLength(150)]
    public string? Email { get; set; }

    [MaxLength(300)]
    public string? Address { get; set; }

    [MaxLength(50)]
    public string? TaxId { get; set; }

    /// <summary>Amount the store currently owes this supplier (store-side payable, mirrors Customer.CurrentBalance).</summary>
    public decimal CurrentBalance { get; set; }

    public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();

    // ---- Supplier Accounting ----
    public ICollection<SupplierPayment> Payments { get; set; } = new List<SupplierPayment>();
    public ICollection<SupplierLedgerEntry> LedgerEntries { get; set; } = new List<SupplierLedgerEntry>();
}

/// <summary>
/// Header for a purchase from a supplier. Receiving this (Status -> Received) is what should
/// trigger ProductStock increases and InventoryTransaction rows, and update Product.CurrentCostPrice.
/// </summary>
public class PurchaseOrder : BaseEntity
{
    [Required, MaxLength(50)]
    public string PurchaseOrderNumber { get; set; } = string.Empty;

    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;

    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;

    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? ReceivedDate { get; set; }

    public decimal TotalCost { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
}

/// <summary>
/// Line item — this is the source of truth for historical cost price per product per purchase,
/// which is what profit-margin reporting (SellingPrice vs UnitCostPrice) is built on.
/// </summary>
public class PurchaseOrderItem : BaseEntity
{
    public int PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int QuantityOrdered { get; set; }
    public int QuantityReceived { get; set; }

    public decimal UnitCostPrice { get; set; }
    public decimal LineTotal { get; set; }
}

/// <summary>
/// A financial payment made to a supplier. Optionally tied to a specific PurchaseOrder,
/// but can also stand alone (e.g. paying down general balance / an old debt).
/// </summary>
public class SupplierPayment : BaseEntity
{
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;

    public int? PurchaseOrderId { get; set; }
    public PurchaseOrder? PurchaseOrder { get; set; }

    public decimal Amount { get; set; }

    public SupplierPaymentMethod PaymentMethod { get; set; }

    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? Notes { get; set; }

    /// <summary>User who recorded the payment (e.g. Owner/Admin/Cashier). Optional for system-generated entries.</summary>
    public int? CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }

    public ICollection<SupplierLedgerEntry> LedgerEntries { get; set; } = new List<SupplierLedgerEntry>();
}

/// <summary>
/// A single line on a Supplier's statement of account (running balance ledger).
/// Written automatically whenever a PurchaseOrder is Received (PurchaseInvoice) or a
/// SupplierPayment is recorded (Payment), and can also be written manually for Adjustment/Refund.
/// </summary>
public class SupplierLedgerEntry : BaseEntity
{
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;

    public SupplierLedgerTransactionType TransactionType { get; set; }

    /// <summary>Signed movement for this entry (positive increases what we owe, negative decreases it).</summary>
    public decimal Amount { get; set; }

    /// <summary>Supplier.CurrentBalance snapshot immediately after this entry was applied.</summary>
    public decimal RunningBalance { get; set; }

    public int? RelatedPurchaseOrderId { get; set; }
    public PurchaseOrder? RelatedPurchaseOrder { get; set; }

    public int? RelatedPaymentId { get; set; }
    public SupplierPayment? RelatedPayment { get; set; }

    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? Notes { get; set; }
}