using IbnAlZumar.API.DTOs.Invoices;
using IbnAlZumar.API.Services.Invoices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IbnAlZumar.API.Controllers;

[ApiController]
[Route("api/invoices")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoicesController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    private int CurrentUserId =>
        int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                  ?? throw new InvalidOperationException("لا يوجد مستخدم مصادق عليه."));

    [HttpPost("from-order")]
    [Authorize]
    public async Task<IActionResult> GenerateFromOrder([FromBody] GenerateOrderInvoiceDto dto, CancellationToken ct)
    {
        var result = await _invoiceService.GenerateFromOrderAsync(dto.OrderId, dto.PrintFormat, CurrentUserId, ct);
        return Ok(result);
    }

    [HttpPost("from-maintenance")]
    [Authorize]
    public async Task<IActionResult> GenerateFromMaintenance([FromBody] GenerateMaintenanceInvoiceDto dto, CancellationToken ct)
    {
        var result = await _invoiceService.GenerateFromMaintenanceRequestAsync(dto.MaintenanceRequestId, dto.PrintFormat, CurrentUserId, ct);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _invoiceService.GetByIdAsync(id, ct);
        return Ok(result);
    }

    [HttpPost("{id:int}/share")]
    [Authorize]
    public async Task<IActionResult> Share(int id, [FromBody] ShareInvoiceRequestDto dto, CancellationToken ct)
    {
        var result = await _invoiceService.ShareAsync(id, dto.ViaWhatsApp, dto.ViaEmail, ct);
        return Ok(result);
    }

    /// <summary>
    /// GET /api/invoices/{id}/pdf?token=... — DELIBERATELY not [Authorize]: WhatsApp's and the
    /// email client's own servers fetch this, not a logged-in browser. Integrity comes from the
    /// signed, short-lived token (InvoiceLinkSigner), same pattern as the Paymob webhook (§5.1).
    /// Never widen this to accept requests without a valid token.
    /// </summary>
    [HttpGet("{id:int}/pdf")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPdf(int id, [FromQuery] string? token, CancellationToken ct)
    {
        var pdfBytes = await _invoiceService.GetPdfBytesIfTokenValidAsync(id, token, ct);
        if (pdfBytes is null) return NotFound();

        return File(pdfBytes, "application/pdf", $"invoice-{id}.pdf");
    }
}

public class ShareInvoiceRequestDto
{
    public bool ViaWhatsApp { get; set; } = true;
    public bool ViaEmail { get; set; } = false;
}
