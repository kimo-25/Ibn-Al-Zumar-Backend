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
    Shipped = 10 // 👈 تم إضافتها بحجم معرّف مستقل لمنع أي تضارب
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
    CustomerCredit = 6
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
    SaleDeducted = 2,
    TransferOut = 3,
    TransferIn = 4,
    AdjustmentIncrease = 5,
    AdjustmentDecrease = 6,
    CustomerReturn = 7,
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