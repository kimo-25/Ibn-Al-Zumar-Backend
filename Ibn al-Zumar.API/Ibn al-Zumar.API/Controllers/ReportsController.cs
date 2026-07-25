using IbnAlZumar.API.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IbnAlZumar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ReportsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // تقرير المبيعات والأرباح الإجمالية
    [HttpGet("sales")]
    public async Task<IActionResult> GetSalesReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var query = _context.Orders.AsNoTracking().AsQueryable();

        if (startDate.HasValue)
            query = query.Where(o => o.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(o => o.CreatedAt <= endDate.Value);

        var totalOrders = await query.CountAsync();
        var totalRevenue = await query.SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

        return Ok(new
        {
            TotalOrders = totalOrders,
            TotalRevenue = totalRevenue,
            StartDate = startDate ?? DateTime.MinValue,
            EndDate = endDate ?? DateTime.UtcNow
        });
    }

    // تقرير حالة المخزون والمنتجات التي تقترب على النفاد
    [HttpGet("inventory-status")]
    public async Task<IActionResult> GetInventoryStatusReport()
    {
        var totalProducts = await _context.Products.CountAsync();

        var lowStockProducts = await _context.Products
            .AsNoTracking()
            .Where(p => p.QuantityPerCarton <= 5) // لو اسم الخاصية عندك Quantity أو Stock جرب استبدالها
            .Select(p => new
            {
                p.Id,
                p.Name,
                Stock = p.QuantityPerCarton,
                Price = p.CurrentCostPrice // لو اسم الخاصية عندك Price أو SellingPrice جرب استبدالها
            })
            .ToListAsync();

        var totalInventoryValue = await _context.Products
            .AsNoTracking()
            .SumAsync(p => p.QuantityPerCarton * p.CurrentCostPrice);

        return Ok(new
        {
            TotalProducts = totalProducts,
            TotalInventoryValue = totalInventoryValue,
            LowStockCount = lowStockProducts.Count,
            LowStockItems = lowStockProducts
        });
    }
}