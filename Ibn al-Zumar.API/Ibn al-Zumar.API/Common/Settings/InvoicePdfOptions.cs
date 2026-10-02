namespace IbnAlZumar.API.Common.Settings;

/// <summary>Bound from appsettings config section "InvoicePdf". See BACKEND_CHANGES_PHASE3.md §2.</summary>
public class InvoicePdfOptions
{
    /// <summary>Folder under the content root where generated PDFs are written, e.g. "App_Data/invoices".</summary>
    public string StorageFolder { get; set; } = "App_Data/invoices";

    /// <summary>
    /// The API's own externally-reachable base URL (e.g. "https://api.ibnalzumar.com"). Required
    /// to build an absolute link for the WhatsApp document header — a background job has no
    /// HttpContext to infer this from, unlike a normal request.
    /// </summary>
    public string PublicBaseUrl { get; set; } = string.Empty;

    /// <summary>HMAC secret for signing the short-lived public /api/invoices/{id}/pdf token. Keep out of source control — see BACKEND_CHANGES_PHASE3.md §6.</summary>
    public string SigningSecret { get; set; } = string.Empty;

    /// <summary>How long a signed PDF link stays valid — long enough for WhatsApp's servers to fetch it, short enough not to be a standing public link.</summary>
    public int LinkExpiryMinutes { get; set; } = 30;
}
