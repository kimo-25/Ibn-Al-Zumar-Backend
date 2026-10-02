using IbnAlZumar.Domain.Entities.Sales;

namespace IbnAlZumar.API.Services.Invoices;

/// <summary>
/// Server-side PDF rendering, needed because WhatsApp document sharing and email attachments
/// both need actual PDF bytes (the existing client-side html2pdf.js/html2canvas pipeline, §5,
/// only ever produces a PDF in the cashier's own browser). Uses QuestPDF — a NEW package
/// reference, not currently in the project; see BACKEND_CHANGES_PHASE3.md §5.
/// </summary>
public interface IInvoicePdfService
{
    /// <summary>Renders the invoice for an Order. `items`/`customerName`/etc. are passed in rather than re-queried, so this stays a pure rendering function.</summary>
    byte[] RenderOrderInvoice(Invoice invoice, string customerName, string? customerPhone, IReadOnlyList<InvoicePdfLineItem> items, decimal subTotal);

    byte[] RenderMaintenanceInvoice(Invoice invoice, string customerName, string? customerPhone, string problemDescription, decimal laborCost, IReadOnlyList<InvoicePdfLineItem> parts);
}

public record InvoicePdfLineItem(string Name, int Quantity, decimal UnitPrice);
