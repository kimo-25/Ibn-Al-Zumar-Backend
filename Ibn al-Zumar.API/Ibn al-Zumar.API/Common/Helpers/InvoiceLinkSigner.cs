using System.Security.Cryptography;
using System.Text;
using IbnAlZumar.API.Common.Settings;
using Microsoft.Extensions.Options;

namespace IbnAlZumar.API.Common.Helpers;

/// <summary>
/// Signs/validates the short-lived token on GET /api/invoices/{id}/pdf?token=... — the one
/// endpoint in the system that must be reachable WITHOUT a login, because WhatsApp's and the
/// email attachment fetcher's servers (not the logged-in cashier's browser) are what actually
/// request the file. Same "HMAC-verified, no [Authorize]" pattern already used for the Paymob
/// webhook callback (§5.1) — integrity via signature instead of a session.
/// </summary>
public class InvoiceLinkSigner
{
    private readonly InvoicePdfOptions _options;

    public InvoiceLinkSigner(IOptions<InvoicePdfOptions> options)
    {
        _options = options.Value;
    }

    public string GenerateToken(int invoiceId)
    {
        var expiryUnix = DateTimeOffset.UtcNow.AddMinutes(_options.LinkExpiryMinutes).ToUnixTimeSeconds();
        var payload = $"{invoiceId}:{expiryUnix}";
        var signature = Sign(payload);
        var token = $"{expiryUnix}.{signature}";
        return Base64UrlEncode(token);
    }

    public bool ValidateToken(int invoiceId, string? token)
    {
        if (string.IsNullOrWhiteSpace(token)) return false;

        try
        {
            var decoded = Base64UrlDecode(token);
            var parts = decoded.Split('.', 2);
            if (parts.Length != 2) return false;

            if (!long.TryParse(parts[0], out var expiryUnix)) return false;
            if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > expiryUnix) return false;

            var expectedSignature = Sign($"{invoiceId}:{expiryUnix}");
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(expectedSignature),
                Encoding.UTF8.GetBytes(parts[1]));
        }
        catch
        {
            return false;
        }
    }

    private string Sign(string payload)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_options.SigningSecret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(hash);
    }

    private static string Base64UrlEncode(string value) =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes(value)).Replace('+', '-').Replace('/', '_').TrimEnd('=');

    private static string Base64UrlDecode(string value)
    {
        var padded = value.Replace('-', '+').Replace('_', '/');
        switch (padded.Length % 4)
        {
            case 2: padded += "=="; break;
            case 3: padded += "="; break;
        }
        return Encoding.UTF8.GetString(Convert.FromBase64String(padded));
    }
}
