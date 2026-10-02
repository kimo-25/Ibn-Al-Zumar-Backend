namespace IbnAlZumar.API.Services.Notifications.WhatsApp;

/// <summary>
/// Transport-only WhatsApp Cloud API client (ARCHITECTURE.md §5.2a: "keep the client
/// transport-only — message text/templating lives in INotificationComposer"). Template
/// messages only — never freeform text.
/// </summary>
public interface IWhatsAppClient
{
    Task<WhatsAppSendResult> SendTemplateAsync(WhatsAppTemplateMessage message, CancellationToken ct = default);
}
