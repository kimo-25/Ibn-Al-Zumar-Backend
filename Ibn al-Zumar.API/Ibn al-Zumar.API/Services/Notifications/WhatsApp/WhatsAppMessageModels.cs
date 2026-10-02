namespace IbnAlZumar.API.Services.Notifications.WhatsApp;

/// <summary>A template-only WhatsApp message. Never a freeform message — see IWhatsAppClient.</summary>
public class WhatsAppTemplateMessage
{
    /// <summary>Raw phone as stored on Customer/User — normalized to E.164 by WhatsAppCloudClient before sending.</summary>
    public string ToPhone { get; set; } = string.Empty;

    public string TemplateName { get; set; } = string.Empty;

    public string? LanguageCode { get; set; }

    /// <summary>Positional {{1}}, {{2}}... body parameters, in order.</summary>
    public List<string> BodyParameters { get; set; } = new();

    /// <summary>Set only for templates with a document header component (e.g. sharing an invoice PDF).</summary>
    public WhatsAppDocumentHeader? DocumentHeader { get; set; }
}

public class WhatsAppDocumentHeader
{
    /// <summary>Publicly fetchable URL — see the signed /api/invoices/{id}/pdf endpoint.</summary>
    public string Url { get; set; } = string.Empty;
    public string Filename { get; set; } = string.Empty;
}

public class WhatsAppSendResult
{
    public bool Success { get; set; }
    public string? ProviderMessageId { get; set; }
    public string? ErrorMessage { get; set; }

    public static WhatsAppSendResult Ok(string providerMessageId) => new() { Success = true, ProviderMessageId = providerMessageId };
    public static WhatsAppSendResult Fail(string error) => new() { Success = false, ErrorMessage = error };
}

/// <summary>
/// Normalizes an Egyptian local number ("01012345678", with or without spaces/dashes/a leading
/// 0020) to E.164 ("+201012345678"). Returns null when the input clearly isn't a sendable
/// Egyptian mobile number — WhatsAppCloudClient treats that as "unverified phone" and refuses to
/// send (ARCHITECTURE.md §5.2a: "never send to an unverified phone").
/// </summary>
public static class EgyptianPhoneNormalizer
{
    public static string? ToE164(string? rawPhone)
    {
        if (string.IsNullOrWhiteSpace(rawPhone)) return null;

        var digits = new string(rawPhone.Where(char.IsDigit).ToArray());

        // Strip a leading international/trunk prefix down to the bare 10-digit local number.
        if (digits.StartsWith("0020")) digits = digits[4..];
        else if (digits.StartsWith("20") && digits.Length > 10) digits = digits[2..];
        else if (digits.StartsWith("0")) digits = digits[1..];

        // Egyptian mobile numbers are 10 digits starting with 1 (e.g. 1012345678).
        if (digits.Length != 10 || digits[0] != '1') return null;

        return $"+20{digits}";
    }
}
