using IbnAlZumar.Domain.Common;
using IbnAlZumar.Domain.Entities.Maintenance;
using IbnAlZumar.Domain.Entities.Sales;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IbnAlZumar.Domain.Entities.Sales;

/// <summary>
/// Phase 3 — implements the Invoice entity sketched in ARCHITECTURE.md §3.3
/// ("If a fiscal/sequential invoice document is required, add Invoice { InvoiceNumber, OrderId,
/// IssuedAt, IssuedByUserId, TaxAmount, TotalAmount, PrintFormat }"). Extended here with:
///  - MaintenanceRequestId (nullable) — Phase 3 also needs to invoice completed repairs, not
///    just Orders; exactly one of OrderId/MaintenanceRequestId is set, never both.
///  - CustomerId (denormalized) — lets the debt/invoice dashboards query without joining through
///    Order or MaintenanceRequest every time.
///  - PdfStoragePath + the two SentVia*At timestamps — needed for the WhatsApp document-template
///    send (which needs a fetchable URL, §1 of BACKEND_CHANGES_PHASE3.md) and for the frontend to
///    show "already shared" instead of re-sending blindly.
/// InvoiceNumber is assigned once, right after the row gets its Id (see InvoiceService), and is
/// never regenerated — same "frozen once issued" rule as Order.TotalAmount (§7.4).
/// </summary>
public class Invoice : BaseEntity
{
    [Required, MaxLength(30)]
    public string InvoiceNumber { get; set; } = string.Empty;

    public int? OrderId { get; set; }
    public Order? Order { get; set; }

    public int? MaintenanceRequestId { get; set; }
    public MaintenanceRequest? MaintenanceRequest { get; set; }

    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

    public int IssuedByUserId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal SubTotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    /// <summary>One of the Phase 1 print-engine format keys — "thermal80" | "thermal58" | "a4" | "a5".</summary>
    [MaxLength(20)]
    public string PrintFormat { get; set; } = "a4";

    /// <summary>Relative storage path of the generated PDF (see IInvoicePdfService), served through the signed /api/invoices/{id}/pdf endpoint.</summary>
    [MaxLength(300)]
    public string? PdfStoragePath { get; set; }

    public DateTime? SentViaWhatsAppAt { get; set; }
    public DateTime? SentViaEmailAt { get; set; }
}
