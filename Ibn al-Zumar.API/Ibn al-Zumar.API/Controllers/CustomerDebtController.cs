using IbnAlZumar.API.Services.Sales;
using IbnAlZumar.Persistence.Seed;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IbnAlZumar.API.Controllers;

// Same namespace caveat as every other Phase 1/2 controller — adjust the `using` if
// DataSeeder.PermissionCodes lives elsewhere, and see BACKEND_CHANGES_PHASE3.md §7 for the new
// permission code this controller expects (`Customers.ManageDebt`).
[ApiController]
[Route("api/customer-debt")]
[Authorize(Policy = DataSeeder.PermissionCodes.CustomersManageDebt)]
public class CustomerDebtController : ControllerBase
{
    private readonly ICustomerDebtService _debtService;

    public CustomerDebtController(ICustomerDebtService debtService)
    {
        _debtService = debtService;
    }

    /// <summary>GET /api/customer-debt?pageNumber=1&amp;pageSize=30 — the debt-tracking dashboard.</summary>
    [HttpGet]
    public async Task<IActionResult> GetDashboard([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 30, CancellationToken ct = default)
    {
        var result = await _debtService.GetDebtDashboardAsync(pageNumber, pageSize, ct);
        return Ok(result);
    }

    /// <summary>POST /api/customer-debt/{customerId}/send-reminder — the dashboard's one-click "send now" button.</summary>
    [HttpPost("{customerId:int}/send-reminder")]
    public async Task<IActionResult> SendReminderNow(int customerId, CancellationToken ct)
    {
        var result = await _debtService.SendReminderNowAsync(customerId, ct);
        return Ok(result);
    }
}
