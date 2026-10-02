namespace IbnAlZumar.API.DTOs.Invoices;

public class InvoiceDto
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public int? OrderId { get; set; }
    public int? MaintenanceRequestId { get; set; }
    public int? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public DateTime IssuedAt { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string PrintFormat { get; set; } = "a4";
    public DateTime? SentViaWhatsAppAt { get; set; }
    public DateTime? SentViaEmailAt { get; set; }
    public string PdfUrl { get; set; } = string.Empty;
}

public class GenerateOrderInvoiceDto
{
    public int OrderId { get; set; }
    public string PrintFormat { get; set; } = "a4";
}

public class GenerateMaintenanceInvoiceDto
{
    public int MaintenanceRequestId { get; set; }
    public string PrintFormat { get; set; } = "a5";
}

public class ShareInvoiceResultDto
{
    public bool WhatsAppSent { get; set; }
    public bool EmailSent { get; set; }
    public string? WhatsAppError { get; set; }
    public string? EmailError { get; set; }
}
