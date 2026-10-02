using IbnAlZumar.API.Common.Settings;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.Api.Services.Email; // IEmailService interface lives here
using IbnAlZumar.API.Services.Notifications.WhatsApp;
using IbnAlZumar.Domain.Entities.Notifications;
using IbnAlZumar.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace IbnAlZumar.API.Services.Notifications;

public class NotificationSender : INotificationSender
{
    private readonly ApplicationDbContext _context;
    private readonly IWhatsAppClient _whatsAppClient;
    private readonly IEmailService _emailService;
    private readonly WhatsAppOptions _whatsAppOptions;
    private readonly ILogger<NotificationSender> _logger;

    public NotificationSender(
        ApplicationDbContext context,
        IWhatsAppClient whatsAppClient,
        IEmailService emailService,
        IOptions<WhatsAppOptions> whatsAppOptions,
        ILogger<NotificationSender> logger)
    {
        _context = context;
        _whatsAppClient = whatsAppClient;
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _whatsAppOptions = whatsAppOptions.Value;
        _logger = logger;
    }

    public async Task<bool> SendWhatsAppTemplateAsync(
        WhatsAppTemplateMessage message,
        string? relatedEntityType = null,
        int? relatedEntityId = null,
        CancellationToken ct = default)
    {
        var log = new NotificationLog
        {
            Channel = NotificationChannel.WhatsApp,
            Recipient = message.ToPhone,
            TemplateName = message.TemplateName,
            PayloadJson = JsonSerializer.Serialize(message),
            Status = NotificationStatus.Pending,
            RelatedEntityType = relatedEntityType,
            RelatedEntityId = relatedEntityId
        };
        _context.NotificationLogs.Add(log);
        await _context.SaveChangesAsync(ct);

        for (var attempt = 1; attempt <= _whatsAppOptions.MaxRetryAttempts; attempt++)
        {
            var result = await _whatsAppClient.SendTemplateAsync(message, ct);
            log.RetryCount = attempt;

            if (result.Success)
            {
                log.Status = NotificationStatus.Sent;
                log.ProviderMessageId = result.ProviderMessageId;
                log.SentAtUtc = DateTime.UtcNow;
                log.Error = null;
                await _context.SaveChangesAsync(ct);
                return true;
            }

            log.Error = result.ErrorMessage;
            _logger.LogWarning("WhatsApp send attempt {Attempt}/{Max} failed for log {LogId}: {Error}",
                attempt, _whatsAppOptions.MaxRetryAttempts, log.Id, result.ErrorMessage);

            if (attempt < _whatsAppOptions.MaxRetryAttempts)
            {
                var delaySeconds = Math.Pow(2, attempt); // 2s, 4s, 8s, ...
                await Task.Delay(TimeSpan.FromSeconds(delaySeconds), ct);
            }
        }

        log.Status = NotificationStatus.Failed;
        await _context.SaveChangesAsync(ct);
        return false;
    }

    public async Task<bool> SendEmailAsync(
        string toEmail,
        string subject,
        string htmlBody,
        string templateName,
        string? relatedEntityType = null,
        int? relatedEntityId = null,
        CancellationToken ct = default)
    {
        var log = new NotificationLog
        {
            Channel = NotificationChannel.Email,
            Recipient = toEmail,
            TemplateName = templateName,
            PayloadJson = JsonSerializer.Serialize(new { subject, htmlBody }),
            Status = NotificationStatus.Pending,
            RelatedEntityType = relatedEntityType,
            RelatedEntityId = relatedEntityId
        };
        _context.NotificationLogs.Add(log);
        await _context.SaveChangesAsync(ct);

        const int maxAttempts = 3;
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                // IEmailService in this solution exposes SendEmailAsync(to, subject, htmlContent)
                await _emailService.SendEmailAsync(toEmail, subject, htmlBody);

                log.Status = NotificationStatus.Sent;
                log.SentAtUtc = DateTime.UtcNow;
                log.RetryCount = attempt;
                log.Error = null;
                await _context.SaveChangesAsync(ct);
                return true;
            }
            catch (Exception ex)
            {
                log.RetryCount = attempt;
                log.Error = ex.Message;
                _logger.LogWarning(ex, "Email send attempt {Attempt}/{Max} failed for log {LogId}", attempt, maxAttempts, log.Id);

                if (attempt < maxAttempts)
                {
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)), ct);
                }
            }
        }

        log.Status = NotificationStatus.Failed;
        await _context.SaveChangesAsync(ct);
        return false;
    }
}
