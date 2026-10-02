using IbnAlZumar.API.DTOs.Catalog;
using IbnAlZumar.Domain.Enums;

namespace IbnAlZumar.API.Services.Catalog;

public interface IPosCatalogService
{
    Task<IbnAlZumar.API.DTOs.Common.PagedResultDto<PosProductDto>> SearchAsync(
        string? query,
        int warehouseId,
        PricingTierType tier,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default);
}
