using IbnAlZumar.Domain.Entities.Sales;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace IbnAlZumar.API.Services.Invoices;

/// <summary>
/// NOTE on PrintFormat: the in-store thermal/A5 receipts already printed live entirely on the
/// frontend (Phase 1's src/utils/print/*, Phase 2's renderMaintenanceReceipt.js) — they're
/// window.print() jobs, not files. This service exists for the case those don't cover: producing
/// an actual PDF FILE to attach to an email or host at a URL for WhatsApp's document header. A
/// letter/A4-style single-page layout is used for that file regardless of Invoice.PrintFormat,
/// since a WhatsApp/email recipient doesn't have a thermal printer on the other end — PrintFormat
/// is kept on the entity for the in-store reprint case (open the same order in the POS and print
/// it again in whatever format), not for this PDF.
/// </summary>
public class InvoicePdfService : IInvoicePdfService
{
    static InvoicePdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] RenderOrderInvoice(Invoice invoice, string customerName, string? customerPhone, IReadOnlyList<InvoicePdfLineItem> items, decimal subTotal)
    {
        return BuildDocument(
            documentTitle: "فاتورة بيع",
            invoiceNumber: invoice.InvoiceNumber,
            issuedAt: invoice.IssuedAt,
            customerName: customerName,
            customerPhone: customerPhone,
            extraInfoLine: null,
            items: items,
            subTotal: subTotal,
            taxAmount: invoice.TaxAmount,
            totalAmount: invoice.TotalAmount);
    }

    public byte[] RenderMaintenanceInvoice(Invoice invoice, string customerName, string? customerPhone, string problemDescription, decimal laborCost, IReadOnlyList<InvoicePdfLineItem> parts)
    {
        var lineItems = new List<InvoicePdfLineItem> { new("المصنعية", 1, laborCost) };
        lineItems.AddRange(parts);
        var subTotal = lineItems.Sum(i => i.UnitPrice * i.Quantity);

        return BuildDocument(
            documentTitle: "فاتورة صيانة",
            invoiceNumber: invoice.InvoiceNumber,
            issuedAt: invoice.IssuedAt,
            customerName: customerName,
            customerPhone: customerPhone,
            extraInfoLine: $"وصف العطل: {problemDescription}",
            items: lineItems,
            subTotal: subTotal,
            taxAmount: invoice.TaxAmount,
            totalAmount: invoice.TotalAmount);
    }

    private static byte[] BuildDocument(
        string documentTitle,
        string invoiceNumber,
        DateTime issuedAt,
        string customerName,
        string? customerPhone,
        string? extraInfoLine,
        IReadOnlyList<InvoicePdfLineItem> items,
        decimal subTotal,
        decimal taxAmount,
        decimal totalAmount)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.ContentFromRightToLeft();
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Column(col =>
                {
                    col.Item().AlignCenter().Text("ابن الزمر — Ibn Al-Zumar").Bold().FontSize(18);
                    col.Item().AlignCenter().Text(documentTitle).FontSize(13);
                    col.Item().PaddingTop(6).Row(row =>
                    {
                        row.RelativeItem().Text($"رقم: {invoiceNumber}");
                        row.RelativeItem().AlignLeft().Text($"{issuedAt:yyyy-MM-dd HH:mm}");
                    });
                    col.Item().PaddingTop(2).LineHorizontal(1);
                });

                page.Content().PaddingTop(15).Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text($"العميل: {customerName}");
                        row.RelativeItem().AlignLeft().Text($"الهاتف: {customerPhone ?? "-"}");
                    });

                    if (!string.IsNullOrWhiteSpace(extraInfoLine))
                    {
                        col.Item().PaddingTop(4).Background(Colors.Grey.Lighten3).Padding(6).Text(extraInfoLine);
                    }

                    col.Item().PaddingTop(12).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(4);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderCell).Text("الصنف");
                            header.Cell().Element(HeaderCell).AlignCenter().Text("الكمية");
                            header.Cell().Element(HeaderCell).AlignCenter().Text("السعر");
                            header.Cell().Element(HeaderCell).AlignCenter().Text("الإجمالي");

                            static IContainer HeaderCell(IContainer c) =>
                                c.Background(Colors.Grey.Lighten2).Padding(6).DefaultTextStyle(x => x.Bold());
                        });

                        foreach (var item in items)
                        {
                            table.Cell().Element(BodyCell).Text(item.Name);
                            table.Cell().Element(BodyCell).AlignCenter().Text(item.Quantity.ToString());
                            table.Cell().Element(BodyCell).AlignCenter().Text(item.UnitPrice.ToString("N2"));
                            table.Cell().Element(BodyCell).AlignCenter().Text((item.UnitPrice * item.Quantity).ToString("N2"));

                            static IContainer BodyCell(IContainer c) =>
                                c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(6);
                        }
                    });

                    col.Item().PaddingTop(15).AlignLeft().Width(220).Column(totals =>
                    {
                        totals.Item().Row(r => { r.RelativeItem().Text("المبلغ الجزئي"); r.RelativeItem().AlignLeft().Text($"{subTotal:N2} ج.م"); });
                        totals.Item().Row(r => { r.RelativeItem().Text("الضريبة"); r.RelativeItem().AlignLeft().Text($"{taxAmount:N2} ج.م"); });
                        totals.Item().PaddingTop(4).LineHorizontal(1);
                        totals.Item().PaddingTop(4).Row(r =>
                        {
                            r.RelativeItem().Text("الإجمالي").Bold().FontSize(13);
                            r.RelativeItem().AlignLeft().Text($"{totalAmount:N2} ج.م").Bold().FontSize(13);
                        });
                    });
                });

                page.Footer().AlignCenter().Text("شكراً لتعاملكم معنا — ابن الزمر").FontSize(9).FontColor(Colors.Grey.Darken1);
            });
        });

        return document.GeneratePdf();
    }
}
