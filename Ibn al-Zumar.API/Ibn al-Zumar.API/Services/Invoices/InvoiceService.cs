using IbnAlZumar.API.Common.Exceptions;
using IbnAlZumar.API.Common.Helpers;
using IbnAlZumar.API.Common.Settings;
using IbnAlZumar.API.DTOs.Invoices;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.API.Services.Notifications;
using IbnAlZumar.Domain.Entities.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace IbnAlZumar.API.Services.Invoices;

public class InvoiceService : IInvoiceService
{
    private readonly ApplicationDbContext _context;
    private readonly IInvoicePdfService _pdfService;
    private readonly INotificationComposer _composer;
    private readonly INotificationSender _sender;
    private readonly InvoiceLinkSigner _linkSigner;
    private readonly InvoicePdfOptions _pdfOptions;

    public InvoiceService(
        ApplicationDbContext context,
        IInvoicePdfService pdfService,
        INotificationComposer composer,
        INotificationSender sender,
        InvoiceLinkSigner linkSigner,
        IOptions<InvoicePdfOptions> pdfOptions)
    {
        _context = context;
        _pdfService = pdfService;
        _composer = composer;
        _sender = sender;
        _linkSigner = linkSigner;
        _pdfOptions = pdfOptions.Value;
    }

    public async Task<InvoiceDto> GenerateFromOrderAsync(int orderId, string printFormat, int issuedByUserId, CancellationToken ct = default)
    {
        var existing = await _context.Invoices.AsNoTracking().FirstOrDefaultAsync(i => i.OrderId == orderId, ct);
        if (existing is not null) return await GetByIdAsync(existing.Id, ct);

        var order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items).ThenInclude(oi => oi.Product) // FIXED: Uses .Items instead of .OrderItems
            .FirstOrDefaultAsync(o => o.Id == orderId, ct)
            ?? throw new NotFoundException("الطلب غير موجود.");

        var invoice = new Invoice
        {
            OrderId = order.Id,
            CustomerId = order.CustomerId,
            IssuedByUserId = issuedByUserId,
            SubTotal = order.SubTotal,
            TaxAmount = order.TaxAmount,
            TotalAmount = order.TotalAmount,
            PrintFormat = printFormat
        };
        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync(ct);

        invoice.InvoiceNumber = $"INV-{invoice.IssuedAt:yyyyMMdd}-{invoice.Id:D6}";

        var lineItems = order.Items
            .Select(oi => new InvoicePdfLineItem(oi.Product?.NameAr ?? oi.Product?.Name ?? "منتج", (int)oi.Quantity, oi.UnitPrice))
            .ToList();

        var pdfBytes = _pdfService.RenderOrderInvoice(
            invoice,
            order.Customer?.FullName ?? "عميل نقدي",
            order.Customer?.Phone,
            lineItems,
            order.SubTotal);

        invoice.PdfStoragePath = await SavePdfAsync(invoice.InvoiceNumber, pdfBytes, ct);
        await _context.SaveChangesAsync(ct);

        return await GetByIdAsync(invoice.Id, ct);
    }

    public async Task<InvoiceDto> GenerateFromMaintenanceRequestAsync(int maintenanceRequestId, string printFormat, int issuedByUserId, CancellationToken ct = default)
    {
        var existing = await _context.Invoices.AsNoTracking().FirstOrDefaultAsync(i => i.MaintenanceRequestId == maintenanceRequestId, ct);
        if (existing is not null) return await GetByIdAsync(existing.Id, ct);

        var request = await _context.MaintenanceRequests
            .Include(m => m.Customer)
            .Include(m => m.PartUsages).ThenInclude(p => p.Product)
            .FirstOrDefaultAsync(m => m.Id == maintenanceRequestId, ct)
            ?? throw new NotFoundException("طلب الصيانة غير موجود.");

        var partsTotal = request.PartUsages.Sum(p => p.LineTotal);
        var subTotal = request.LaborCost + partsTotal;

        var invoice = new Invoice
        {
            MaintenanceRequestId = request.Id,
            CustomerId = request.CustomerId,
            IssuedByUserId = issuedByUserId,
            SubTotal = subTotal,
            TaxAmount = 0m,
            TotalAmount = subTotal,
            PrintFormat = printFormat
        };
        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync(ct);

        invoice.InvoiceNumber = $"INV-MNT-{invoice.IssuedAt:yyyyMMdd}-{invoice.Id:D6}";

        var partLineItems = request.PartUsages
            .Select(p => new InvoicePdfLineItem(p.Product?.NameAr ?? p.Product?.Name ?? "قطعة غيار", p.Quantity, p.UnitCostPrice))
            .ToList();

        var pdfBytes = _pdfService.RenderMaintenanceInvoice(
            invoice,
            request.Customer?.FullName ?? "عميل نقدي",
            request.Customer?.Phone,
            request.ProblemDescription,
            request.LaborCost,
            partLineItems);

        invoice.PdfStoragePath = await SavePdfAsync(invoice.InvoiceNumber, pdfBytes, ct);
        await _context.SaveChangesAsync(ct);

        return await GetByIdAsync(invoice.Id, ct);
    }

    public async Task<InvoiceDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var invoice = await _context.Invoices
            .AsNoTracking()
            .Include(i => i.Customer)
            .FirstOrDefaultAsync(i => i.Id == id, ct)
            ?? throw new NotFoundException("الفاتورة غير موجودة.");

        return MapToDto(invoice);
    }

    public async Task<byte[]?> GetPdfBytesIfTokenValidAsync(int id, string? token, CancellationToken ct = default)
    {
        if (!_linkSigner.ValidateToken(id, token)) return null;

        var invoice = await _context.Invoices.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id, ct);
        if (invoice?.PdfStoragePath is null) return null;

        var fullPath = Path.Combine(AppContext.BaseDirectory, invoice.PdfStoragePath);
        return File.Exists(fullPath) ? await File.ReadAllBytesAsync(fullPath, ct) : null;
    }

    public async Task<ShareInvoiceResultDto> ShareAsync(int id, bool viaWhatsApp, bool viaEmail, CancellationToken ct = default)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Customer)
            .FirstOrDefaultAsync(i => i.Id == id, ct)
            ?? throw new NotFoundException("الفاتورة غير موجودة.");

        if (invoice.PdfStoragePath is null)
            throw new BadRequestException("لم يتم إنشاء ملف PDF لهذه الفاتورة بعد.");

        var customerName = invoice.Customer?.FullName ?? "عميل نقدي";
        var result = new ShareInvoiceResultDto();

        if (viaWhatsApp)
        {
            if (string.IsNullOrWhiteSpace(invoice.Customer?.Phone))
            {
                result.WhatsAppError = "لا يوجد رقم هاتف مسجل لهذا العميل.";
            }
            else
            {
                var documentUrl = BuildPublicPdfUrl(invoice.Id);
                var message = _composer.ComposeInvoiceShareWhatsApp(invoice, customerName, documentUrl);
                message.ToPhone = invoice.Customer.Phone;

                result.WhatsAppSent = await _sender.SendWhatsAppTemplateAsync(message, "Invoice", invoice.Id, ct);
                if (result.WhatsAppSent)
                {
                    invoice.SentViaWhatsAppAt = DateTime.UtcNow;
                }
                else
                {
                    result.WhatsAppError = "تعذر الإرسال عبر واتساب بعد عدة محاولات.";
                }
            }
        }

        if (viaEmail)
        {
            if (string.IsNullOrWhiteSpace(invoice.Customer?.Email))
            {
                result.EmailError = "لا يوجد بريد إلكتروني مسجل لهذا العميل.";
            }
            else
            {
                var (subject, htmlBody) = _composer.ComposeInvoiceShareEmail(invoice, customerName);
                result.EmailSent = await _sender.SendEmailAsync(invoice.Customer.Email, subject, htmlBody, "invoice_share", "Invoice", invoice.Id, ct);
                if (result.EmailSent)
                {
                    invoice.SentViaEmailAt = DateTime.UtcNow;
                }
                else
                {
                    result.EmailError = "تعذر الإرسال عبر البريد الإلكتروني بعد عدة محاولات.";
                }
            }
        }

        await _context.SaveChangesAsync(ct);
        return result;
    }

    private async Task<string> SavePdfAsync(string invoiceNumber, byte[] pdfBytes, CancellationToken ct)
    {
        var folder = Path.Combine(AppContext.BaseDirectory, _pdfOptions.StorageFolder);
        Directory.CreateDirectory(folder);

        var relativePath = Path.Combine(_pdfOptions.StorageFolder, $"{invoiceNumber}.pdf");
        var fullPath = Path.Combine(AppContext.BaseDirectory, relativePath);
        await File.WriteAllBytesAsync(fullPath, pdfBytes, ct);

        return relativePath;
    }

    private string BuildPublicPdfUrl(int invoiceId)
    {
        var token = _linkSigner.GenerateToken(invoiceId);
        return $"{_pdfOptions.PublicBaseUrl.TrimEnd('/')}/api/invoices/{invoiceId}/pdf?token={Uri.EscapeDataString(token)}";
    }

    private static InvoiceDto MapToDto(Invoice invoice)
    {
        return new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            OrderId = invoice.OrderId,
            MaintenanceRequestId = invoice.MaintenanceRequestId,
            CustomerId = invoice.CustomerId,
            CustomerName = invoice.Customer?.FullName,
            CustomerPhone = invoice.Customer?.Phone,
            IssuedAt = invoice.IssuedAt,
            SubTotal = invoice.SubTotal,
            TaxAmount = invoice.TaxAmount,
            TotalAmount = invoice.TotalAmount,
            PrintFormat = invoice.PrintFormat,
            SentViaWhatsAppAt = invoice.SentViaWhatsAppAt,
            SentViaEmailAt = invoice.SentViaEmailAt,
            PdfUrl = $"/api/invoices/{invoice.Id}/pdf"
        };
    }
}