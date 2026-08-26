using Microsoft.AspNetCore.Http;

namespace IbnAlZumar.API.Services.Catalog;

public sealed record InvoiceExcelFile(byte[] Content, string FileName);

public interface IInvoiceToExcelService
{
    Task<InvoiceExcelFile> ConvertAsync(IFormFile file, CancellationToken cancellationToken = default);
}