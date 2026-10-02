namespace IbnAlZumar.API.Common.Settings;

/// <summary>Bound from appsettings config section "WhatsApp". See BACKEND_CHANGES_PHASE3.md §2.</summary>
public class WhatsAppOptions
{
    public string BaseUrl { get; set; } = "https://graph.facebook.com/v20.0";
    public string PhoneNumberId { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string DefaultLanguage { get; set; } = "ar";

    /// <summary>Max send attempts before a NotificationLog row is marked Failed for good.</summary>
    public int MaxRetryAttempts { get; set; } = 3;
}
