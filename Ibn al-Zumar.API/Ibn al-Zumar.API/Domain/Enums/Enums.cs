namespace IbnAlZumar.Domain.Enums;

/// <summary>Where an order originated from.</summary>
public enum OrderSource
{
    Online = 1,
    InStore = 2
}

public enum OrderStatus
{
    PendingConfirmation = 1,
    Confirmed = 2,
    Processing = 3,
    ReadyForPickup = 4,
    OutForDelivery = 5,
    Delivered = 6,
    Completed = 7,
    Cancelled = 8,
    Returned = 9,
    Shipped = 10,
    CancellationRequested = 11 // تمت الإضافة لطلب الإلغاء
}

/// <summary>
/// CustomerCredit represents a sale on debt ("الشكك") — increases the customer's CurrentBalance
/// instead of collecting cash at the time of sale.
/// </summary>
public enum PaymentMethod
{
    CashOnDelivery = 1,
    Cash = 2,
    CreditCard = 3,
    InstaPay = 4,
    Fawry = 5,
    CustomerCredit = 6,
    Wallet = 7
}

public enum PaymentStatus
{
    Pending = 1,
    Paid = 2,
    Failed = 3,
    CodPending = 4
}

/// <summary>
/// Method used to pay a supplier. Kept separate from Sales PaymentMethod
/// so Purchasing accounting can evolve independently (e.g. Cheque is supplier-specific).
/// </summary>
public enum SupplierPaymentMethod
{
    Cash = 1,
    BankTransfer = 2,
    Cheque = 3
}

/// <summary>
/// Nature of a movement on a Supplier's statement of account.
/// </summary>
public enum SupplierLedgerTransactionType
{
    /// <summary>Supplier invoiced us (Purchase Order received) — increases what we owe.</summary>
    PurchaseInvoice = 1,

    /// <summary>We paid the supplier — decreases what we owe.</summary>
    Payment = 2,

    /// <summary>Manual correction to the balance (damaged goods, pricing error, etc.).</summary>
    Adjustment = 3,

    /// <summary>Supplier refunded us — decreases what we owe.</summary>
    Refund = 4
}

public enum DiscountType
{
    None = 0,
    Percentage = 1,
    FixedAmount = 2
}

public enum PurchaseOrderStatus
{
    Draft = 1,
    Ordered = 2,
    PartiallyReceived = 3,
    Received = 4,
    Cancelled = 5
}

/// <summary>Every stock movement in or out of a warehouse is logged with one of these types.</summary>
public enum InventoryTransactionType
{
    PurchaseReceived = 1,
    Purchase = 1,          // مرادف جديد متوافق مع الكود الجديد
    SaleDeducted = 2,
    Sale = 2,              // مرادف جديد متوافق مع الكود الجديد
    TransferOut = 3,
    TransferIn = 4,
    AdjustmentIncrease = 5,
    Adjustment = 5,        // مرادف جديد متوافق مع الكود الجديد
    AdjustmentDecrease = 6,
    CustomerReturn = 7,
    Return = 7,            // مرادف جديد متوافق مع الكود الجديد
    SupplierReturn = 8
}

public enum StockTransferStatus
{
    Requested = 1,
    InTransit = 2,
    Completed = 3,
    Cancelled = 4
}

/// <summary>
/// Drives the customer debt ledger ("الشكك"): SaleOnCredit increases CurrentBalance,
/// PaymentReceived decreases it.
/// </summary>
public enum LedgerTransactionType
{
    SaleOnCredit = 1,
    PaymentReceived = 2,
    ManualAdjustment = 3
}

/// <summary>Tells the UI/API how to parse and render a dynamic product attribute's Value string.</summary>
public enum AttributeDataType
{
    Text = 1,
    Number = 2,
    Boolean = 3
}

/// <summary>Determines whether the reminder is a Quranic Ayah or an Islamic Dhikr.</summary>
public enum ReminderType
{
    Quran = 1,
    Dhikr = 2
}

// ================= قسم الصيانة =================
public enum MaintenanceStatus
{
    Pending = 1,          // قيد المراجعة
    Priced = 2,           // تم التسعير (في انتظار موافقة العميل)
    Approved = 3,         // العميل وافق
    Rejected = 4,         // مرفوض
    Completed = 5         // تم الانتهاء
}

public enum DeliveryMethod
{
    CustomerDropOff = 1,  // العميل هيجيب الجهاز المحل
    CompanyPickup = 2     // الشركة هتبعت مندوب
}

// ================= تمت الإضافة لطلب مناطق الشحن الجديدة =================
public enum CustomZoneRequestStatus
{
    None = 0,
    Pending = 1,
    Approved = 2,
    Rejected = 3
}