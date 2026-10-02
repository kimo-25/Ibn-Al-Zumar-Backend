using IbnAlZumar.API.Services.Notifications.WhatsApp;

namespace IbnAlZumar.API.Services.Notifications;

/// <summary>
/// One layer above the transport clients (WhatsAppCloudClient / IEmailService): every call here
/// writes a NotificationLog row first, retries with exponential backoff on failure (up to
/// WhatsAppOptions.MaxRetryAttempts), and leaves the log row as the durable record of what was
/// actually sent, to whom, and whether it worked (ARCHITECTURE.md §5.2a).
/// </summary>
public interface INotificationSender
{
    Task<bool> SendWhatsAppTemplateAsync(
        WhatsAppTemplateMessage message,
        string? relatedEntityType = null,
        int? relatedEntityId = null,
        CancellationToken ct = default);

    Task<bool> SendEmailAsync(
        string toEmail,
        string subject,
        string htmlBody,
        string templateName,
        string? relatedEntityType = null,
        int? relatedEntityId = null,
        CancellationToken ct = default);
}
