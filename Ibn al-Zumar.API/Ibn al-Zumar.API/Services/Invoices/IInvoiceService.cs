using IbnAlZumar.API.DTOs.Invoices;

namespace IbnAlZumar.API.Services.Invoices;

public interface IInvoiceService
{
    Task<InvoiceDto> GenerateFromOrderAsync(int orderId, string printFormat, int issuedByUserId, CancellationToken ct = default);

    Task<InvoiceDto> GenerateFromMaintenanceRequestAsync(int maintenanceRequestId, string printFormat, int issuedByUserId, CancellationToken ct = default);

    Task<InvoiceDto> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>Returns the raw PDF bytes IF the signed token is valid — used by the public, unauthenticated /api/invoices/{id}/pdf endpoint.</summary>
    Task<byte[]?> GetPdfBytesIfTokenValidAsync(int id, string? token, CancellationToken ct = default);

    /// <summary>Sends via whichever of WhatsApp/Email the customer has a usable contact for; records SentVia*At.</summary>
    Task<ShareInvoiceResultDto> ShareAsync(int id, bool viaWhatsApp, bool viaEmail, CancellationToken ct = default);
}
