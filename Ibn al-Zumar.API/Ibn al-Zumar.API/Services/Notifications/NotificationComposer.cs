using IbnAlZumar.Domain.Entities.Sales;
using IbnAlZumar.API.Services.Notifications.WhatsApp;

namespace IbnAlZumar.API.Services.Notifications;

public class NotificationComposer : INotificationComposer
{
    public WhatsAppTemplateMessage ComposeDebtReminderWhatsApp(Customer customer, decimal balance, DateTime? lastPaymentDate)
    {
        var lastPaymentText = lastPaymentDate.HasValue
            ? lastPaymentDate.Value.ToString("yyyy-MM-dd")
            : "لا توجد دفعات سابقة";

        // WhatsApp template "debt_reminder" (ARCHITECTURE.md §5.2a) — positional body params in
        // the order the approved Meta template defines them. Adjust the order here if your
        // approved template's placeholder order differs; this is the ONLY place that matters.
        return new WhatsAppTemplateMessage
        {
            ToPhone = customer.Phone ?? string.Empty,
            TemplateName = "debt_reminder",
            LanguageCode = "ar",
            BodyParameters = new List<string>
            {
                customer.FullName,
                balance.ToString("N2"),
                lastPaymentText
            }
        };
    }

    public (string Subject, string HtmlBody) ComposeDebtReminderEmail(Customer customer, decimal balance, DateTime? lastPaymentDate)
    {
        var lastPaymentText = lastPaymentDate.HasValue ? lastPaymentDate.Value.ToString("yyyy-MM-dd") : "لا توجد دفعات سابقة";

        var subject = "تذكير برصيد مستحق — ابن الزمر";
        var body = $@"
            <div dir='rtl' style='font-family: Cairo, Arial, sans-serif; color:#111;'>
                <p>مرحباً {System.Net.WebUtility.HtmlEncode(customer.FullName)}،</p>
                <p>نود تذكيركم بوجود رصيد مستحق بقيمة <strong>{balance:N2} ج.م</strong> لدى ابن الزمر.</p>
                <p>تاريخ آخر دفعة: {lastPaymentText}</p>
                <p>يرجى التكرم بسداد المبلغ في أقرب وقت ممكن. لأي استفسار، يرجى التواصل معنا.</p>
                <p>شكراً لتعاملكم معنا.</p>
            </div>";

        return (subject, body);
    }

    public WhatsAppTemplateMessage ComposeInvoiceShareWhatsApp(Invoice invoice, string customerName, string documentUrl)
    {
        // WhatsApp template "invoice_share" — a document-header template (add it in the Meta
        // Business Manager alongside the six templates already listed in §5.2a; it isn't one of
        // the original six, but follows the exact same "template-only" rule).
        return new WhatsAppTemplateMessage
        {
            ToPhone = string.Empty, // filled by the caller (InvoiceService) from Customer.Phone
            TemplateName = "invoice_share",
            LanguageCode = "ar",
            BodyParameters = new List<string>
            {
                customerName,
                invoice.InvoiceNumber,
                invoice.TotalAmount.ToString("N2")
            },
            DocumentHeader = new WhatsAppDocumentHeader
            {
                Url = documentUrl,
                Filename = $"{invoice.InvoiceNumber}.pdf"
            }
        };
    }

    public (string Subject, string HtmlBody) ComposeInvoiceShareEmail(Invoice invoice, string customerName)
    {
        var subject = $"فاتورتكم رقم {invoice.InvoiceNumber} — ابن الزمر";
        var body = $@"
            <div dir='rtl' style='font-family: Cairo, Arial, sans-serif; color:#111;'>
                <p>مرحباً {System.Net.WebUtility.HtmlEncode(customerName)}،</p>
                <p>مرفق فاتورتكم رقم <strong>{invoice.InvoiceNumber}</strong> بقيمة إجمالية <strong>{invoice.TotalAmount:N2} ج.م</strong>.</p>
                <p>شكراً لتعاملكم معنا.</p>
            </div>";

        return (subject, body);
    }
}
