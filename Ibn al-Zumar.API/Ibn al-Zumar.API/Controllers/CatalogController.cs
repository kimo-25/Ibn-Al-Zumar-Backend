using System.Security.Claims;
using IbnAlZumar.API.Services.Ai;
using IbnAlZumar.API.Services.Catalog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IbnAlZumar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CatalogController : ControllerBase
{
    private readonly IInvoiceToExcelService _invoiceService;
    private readonly IAiAuditLogService _audit;

    public CatalogController(IInvoiceToExcelService invoiceService, IAiAuditLogService audit)
    {
        _invoiceService = invoiceService;
        _audit = audit;
    }

    [HttpPost("convert-invoice-to-excel")]
    [Authorize(Roles = "Admin,SuperAdmin,STORE_OWNER")]
    [Consumes("multipart/form-data")]
    [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    public async Task<IActionResult> ConvertInvoiceToExcel(IFormFile file, CancellationToken cancellationToken)
    {
        var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role || c.Type == "role").Select(c => c.Value).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        var email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email");
        try
        {
            var result = await _invoiceService.ConvertAsync(file, cancellationToken);
            await _audit.LogAsync(new AiAuditEntry(null, email, roles, "invoice_to_excel", file?.FileName, Succeeded: true, IpAddress: HttpContext.Connection.RemoteIpAddress?.ToString()), cancellationToken);
            return File(result.Content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.FileName);
        }
        catch (ArgumentException ex)
        {
            await _audit.LogAsync(new AiAuditEntry(null, email, roles, "invoice_to_excel", file?.FileName, Succeeded: false, Error: ex.Message, IpAddress: HttpContext.Connection.RemoteIpAddress?.ToString()), cancellationToken);
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            await _audit.LogAsync(new AiAuditEntry(null, email, roles, "invoice_to_excel", file?.FileName, Succeeded: false, Error: ex.Message, IpAddress: HttpContext.Connection.RemoteIpAddress?.ToString()), cancellationToken);
            return StatusCode(StatusCodes.Status502BadGateway, new { message = "تعذر استخراج بيانات الفاتورة حالياً." });
        }
    }
}