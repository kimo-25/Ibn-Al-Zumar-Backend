// NOTE: per ARCHITECTURE.md §4.3, `PagedResultDto<T>` is already a standing convention used by
// existing paged endpoints (e.g. GET /api/Products, consumed by PosCheckoutPage.jsx's
// `response.data.items` / `response.data.totalPages`). Grep the repo for an existing
// `PagedResultDto` class first — if one already exists, DO NOT add this file (delete it and
// reuse the existing type instead, adjusting property names in PosCatalogService if they differ,
// e.g. `Page` vs `PageNumber`). This file only exists so PosCatalogService compiles standalone
// if no such shared DTO is found.

namespace IbnAlZumar.API.DTOs.Common;

public class PagedResultDto<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
