using IbnAlZumar.Domain.Entities.Sales;
using IbnAlZumar.API.Services.Notifications.WhatsApp;

namespace IbnAlZumar.API.Services.Notifications;

/// <summary>
/// All message composition (WhatsApp template params AND email subject/body) lives here —
/// WhatsAppCloudClient and the Email service stay pure transport (§5.2a). Changing wording,
/// adding a language, or adjusting which fields appear in a reminder is a one-file change.
/// </summary>
public interface INotificationComposer
{
    WhatsAppTemplateMessage ComposeDebtReminderWhatsApp(Customer customer, decimal balance, DateTime? lastPaymentDate);

    (string Subject, string HtmlBody) ComposeDebtReminderEmail(Customer customer, decimal balance, DateTime? lastPaymentDate);

    WhatsAppTemplateMessage ComposeInvoiceShareWhatsApp(Invoice invoice, string customerName, string documentUrl);

    (string Subject, string HtmlBody) ComposeInvoiceShareEmail(Invoice invoice, string customerName);
}
