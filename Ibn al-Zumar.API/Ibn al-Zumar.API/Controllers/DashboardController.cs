using IbnAlZumar.API.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IbnAlZumar.API.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Owner hub summary: restricted to Owner only (Super Admin/Owner role).
    [HttpGet("owner/summary")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> GetOwnerSummary()
    {
        // Basic safe aggregates — adjust field names if your domain differs.
        var totalRevenue = await _context.Orders.AsNoTracking().SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;
        var totalOrders = await _context.Orders.AsNoTracking().CountAsync();
        var totalCustomers = await _context.Customers.AsNoTracking().CountAsync();
        var totalProducts = await _context.Products.AsNoTracking().CountAsync();

        // Low-stock summary: real current stock (sum of ProductStock.Quantity) vs. MinStockThreshold,
        // instead of the previous placeholder that compared against QuantityPerCarton.
        var lowStockQuery = _context.Products.AsNoTracking()
            .Where(p => p.TrackInventory && p.IsActive)
            .Select(p => new
            {
                CurrentStock = p.Stocks.Sum(s => (int?)s.QuantityOnHand) ?? 0,
                p.MinStockThreshold
            })
            .Where(p => p.CurrentStock <= p.MinStockThreshold);

        var lowStock = await lowStockQuery.CountAsync();
        var outOfStock = await lowStockQuery.CountAsync(p => p.CurrentStock <= 0);

        return Ok(new
        {
            TotalRevenue = totalRevenue,
            TotalOrders = totalOrders,
            TotalCustomers = totalCustomers,
            TotalProducts = totalProducts,
            LowStockCount = lowStock,
            OutOfStockCount = outOfStock
        });
    }

    // Operations hub summary: Owner and Admin allowed. Moderator is intentionally excluded.
    [HttpGet("operations/summary")]
    [Authorize(Roles = "Owner, Admin")]
    public async Task<IActionResult> GetOperationsSummary()
    {
        var today = DateTime.UtcNow.Date;
        var ordersToday = await _context.Orders.AsNoTracking()
            .Where(o => o.CreatedAt >= today)
            .CountAsync();

        // pending/processing status may differ in your model; attempt to count by a common Status property if present.
        int pending = 0;
        try
        {
            pending = await _context.Orders.AsNoTracking()
                .Where(o => EF.Property<string>(o, "Status") == "Pending")
                .CountAsync();
        }
        catch
        {
            // ignore if Status property does not exist in the model
        }

        var recentOrders = await _context.Orders.AsNoTracking()
            .OrderByDescending(o => o.CreatedAt)
            .Take(10)
            .Select(o => new { o.Id, o.TotalAmount, o.CreatedAt })
            .ToListAsync();

        // Same low-stock aggregate as the owner summary, exposed here too so the
        // Operations hub can badge the "ÊäÈíåÇÊ ÇáäæÇÞÕ" tab without an extra round trip.
        var lowStock = await _context.Products.AsNoTracking()
            .Where(p => p.TrackInventory && p.IsActive)
            .Select(p => new
            {
                CurrentStock = p.Stocks.Sum(s => (int?)s.QuantityOnHand) ?? 0,
                p.MinStockThreshold
            })
            .CountAsync(p => p.CurrentStock <= p.MinStockThreshold);

        return Ok(new
        {
            OrdersToday = ordersToday,
            PendingOrders = pending,
            RecentOrders = recentOrders,
            LowStockCount = lowStock
        });
    }
}