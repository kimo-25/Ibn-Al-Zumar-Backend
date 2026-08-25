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
        var query = _context.Orders.AsNoTracking().Where(o => !o.IsDeleted);

        if (startDate.HasValue)
            query = query.Where(o => o.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(o => o.CreatedAt <= endDate.Value);

        var totalOrders = await query.CountAsync();
        var totalRevenue = await query.SumAsync(o => (decimal?)o.TotalAmount) ?? 0;
        var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

        return Ok(new
        {
            TotalOrders = totalOrders,
            TotalRevenue = totalRevenue,
            AverageOrderValue = Math.Round(averageOrderValue, 2),
            StartDate = startDate ?? DateTime.MinValue,
            EndDate = endDate ?? DateTime.UtcNow
        });
    }

    // تقرير حالة المخزون والمنتجات التي تقترب على النفاد
    [HttpGet("inventory-status")]
    public async Task<IActionResult> GetInventoryStatusReport()
    {
        var totalProducts = await _context.Products.AsNoTracking().CountAsync(p => p.IsActive);

        var productsWithStock = await _context.Products
            .AsNoTracking()
            .Where(p => p.IsActive)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.NameAr,
                p.SKU,
                TotalStock = p.Stocks.Sum(s => (int?)s.QuantityOnHand) ?? 0,
                MinStock = p.MinStockThreshold,
                CostPrice = p.CurrentCostPrice
            })
            .ToListAsync();

        var lowStockItems = productsWithStock
            .Where(p => p.TotalStock > 0 && p.TotalStock <= p.MinStock)
            .ToList();

        var outOfStockItems = productsWithStock
            .Where(p => p.TotalStock == 0)
            .ToList();

        var totalInventoryValue = productsWithStock
            .Sum(p => (decimal)p.TotalStock * p.CostPrice);

        return Ok(new
        {
            TotalProducts = totalProducts,
            TotalInventoryValue = totalInventoryValue,
            LowStockCount = lowStockItems.Count,
            OutOfStockCount = outOfStockItems.Count,
            LowStockItems = lowStockItems
        });
    }

    // تقرير التقييم المالي والأرباح
    [HttpGet("financial")]
    public async Task<IActionResult> GetFinancialReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var query = _context.Orders.AsNoTracking().Where(o => !o.IsDeleted);

        if (startDate.HasValue)
            query = query.Where(o => o.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(o => o.CreatedAt <= endDate.Value);

        var totalRevenue = await query.SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

        // حساب الأرباح التقديرية بناءً على إجمالي المبيعات وقيمة المخزون
        var estimatedNetProfit = totalRevenue * 0.25m; // نسبة تشغيلية كمثال

        return Ok(new
        {
            TotalRevenue = totalRevenue,
            EstimatedNetProfit = estimatedNetProfit,
            TaxEstimated = totalRevenue * 0.14m
        });
    }
}