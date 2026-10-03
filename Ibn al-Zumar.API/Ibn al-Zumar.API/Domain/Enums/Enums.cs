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

// --- Notification enums required by NotificationLog & NotificationSender ---
public enum NotificationChannel
{
    WhatsApp = 1,
    Email = 2,
    Sms = 3
}

public enum NotificationStatus
{
    Pending = 1,
    Sent = 2,
    Failed = 3
}

/// <summary>
/// CustomerCredit represents a sale on debt ("الشكك") — increases the customer's CurrentBalance
/// instead of collecting cash at the time of sale.
/// </summary>
public enum PaymentMethod
{
    Cash = 0,
    CashOnDelivery = 1,
    CreditCard = 2,
    InstaPay = 3,
    Wallet = 4,
    Fawry = 5,
    ApplePay = 8
}

public enum PaymentStatus
{
    Pending = 1,
    Paid = 2,
    Failed = 3,
    CodPending = 4
}

public enum SupplierPaymentMethod
{
    Cash = 1,
    BankTransfer = 2,
    Cheque = 3
}

public enum SupplierLedgerTransactionType
{
    PurchaseInvoice = 1,
    Payment = 2,
    Adjustment = 3,
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

public enum InventoryTransactionType
{
    PurchaseReceived = 1,
    PurchaseReceive = 1,
    Purchase = 1,

    SaleDeducted = 2,
    SalesDeduct = 2,
    Sale = 2,

    TransferOut = 3,
    TransferIn = 4,

    AdjustmentIncrease = 5,
    Adjustment = 5,

    AdjustmentDecrease = 6,

    CustomerReturn = 7,
    Return = 7,
    BatchReceived = 7,

    SupplierReturn = 8,
    BatchConsumed = 8,

    // Phase 2 Spare Part Consumption
    MaintenanceUsed = 9
}

public enum StockTransferStatus
{
    Requested = 1,
    InTransit = 2,
    Completed = 3,
    Cancelled = 4
}

public enum TotalSaleType
{
    Invoice = 1,
    Return = 2
}

public enum InventoryAdjustmentStatus
{
    Pending = 1,
    Completed = 2,
    Cancelled = 3
}

// --- Added enums to fix CS0246 / CS0103 missing-type errors across the solution ---

public enum MaintenanceStatus
{
    Pending = 1,
    Priced = 2,
    Approved = 3,
    Rejected = 4,
    Completed = 5,
    // Phase 2 repair-shop states
    InDiagnostics = 6,
    AwaitingParts = 7,
    InRepair = 8,
    Delivered = 9,
    Cancelled = 10
}

public enum DeliveryMethod
{
    CustomerDropOff = 1,
    CompanyPickup = 2
}

public enum ReminderType
{
    Quran = 1,
    Dhikr = 2
}

public enum WarehouseTier
{
    MainCentral = 1,
    RegionalBranch = 2,
    PosShelfLocation = 3
}

public enum AttributeDataType
{
    Text = 1,
    Number = 2,
    Boolean = 3
}

public enum CustomZoneRequestStatus
{
    None = 0,
    Pending = 1,
    Approved = 2,
    Rejected = 3
}

public enum LedgerTransactionType
{
    SaleOnCredit = 1,
    PaymentReceived = 2,
    ManualAdjustment = 3
}

public enum PricingTierType
{
    Retail = 1,
    Wholesale = 2,
    FirstWholesale = 3,
    Distributor = 4
}