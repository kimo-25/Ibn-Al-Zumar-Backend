using IbnAlZumar.API.DTOs.Maintenance;
using IbnAlZumar.API.Services.Maintenance;
using IbnAlZumar.Domain.Enums;
using IbnAlZumar.Persistence.Seed;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IbnAlZumar.API.Controllers;

// NOTE: lives alongside the existing MaintenanceController (which keeps handling the online
// customer-inquiry intake + Pending/Priced/Approved/Rejected/Completed pricing screen exactly as
// it does today) rather than replacing it — this is the in-store repair-shop layer on top. Same
// namespace caveat as the Phase 1 controllers: adjust the `using` if DataSeeder.PermissionCodes
// lives elsewhere in your copy of the repo, and see BACKEND_CHANGES_PHASE2.md §4 for the two new
// permission codes (Maintenance.View / Maintenance.Manage) this controller expects to exist.
[ApiController]
[Route("api/maintenance-workflow")]
[Authorize]
public class MaintenanceWorkflowController : ControllerBase
{
    private readonly IMaintenanceWorkflowService _workflowService;

    public MaintenanceWorkflowController(IMaintenanceWorkflowService workflowService)
    {
        _workflowService = workflowService;
    }

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? throw new InvalidOperationException("لا يوجد مستخدم مصادق عليه."));

    /// <summary>GET /api/maintenance-workflow?status=&amp;technicianId=&amp;pageNumber=1&amp;pageSize=30 — powers both the Kanban and table views.</summary>
    [HttpGet]
    [Authorize(Policy = DataSeeder.PermissionCodes.MaintenanceView)]
    public async Task<IActionResult> GetList(
        [FromQuery] MaintenanceStatus? status,
        [FromQuery] int? technicianId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 30,
        CancellationToken ct = default)
    {
        var result = await _workflowService.GetListAsync(status, technicianId, pageNumber, pageSize, ct);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = DataSeeder.PermissionCodes.MaintenanceView)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _workflowService.GetByIdAsync(id, ct);
        return Ok(result);
    }

    [HttpPost("{id:int}/assign-technician")]
    [Authorize(Policy = DataSeeder.PermissionCodes.MaintenanceManage)]
    public async Task<IActionResult> AssignTechnician(int id, [FromBody] AssignTechnicianDto dto, CancellationToken ct)
    {
        var result = await _workflowService.AssignTechnicianAsync(id, dto.TechnicianUserId, CurrentUserId, ct);
        return Ok(result);
    }

    [HttpPost("{id:int}/status")]
    [Authorize(Policy = DataSeeder.PermissionCodes.MaintenanceManage)]
    public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeMaintenanceStatusDto dto, CancellationToken ct)
    {
        var result = await _workflowService.ChangeStatusAsync(id, dto.NewStatus, CurrentUserId, dto.Note, ct);
        return Ok(result);
    }

    [HttpPost("{id:int}/notes")]
    [Authorize(Policy = DataSeeder.PermissionCodes.MaintenanceManage)]
    public async Task<IActionResult> AddNote(int id, [FromBody] AddMaintenanceNoteDto dto, CancellationToken ct)
    {
        var result = await _workflowService.AddNoteAsync(id, CurrentUserId, dto.Note, ct);
        return Ok(result);
    }

    [HttpPost("{id:int}/parts")]
    [Authorize(Policy = DataSeeder.PermissionCodes.MaintenanceManage)]
    public async Task<IActionResult> AddPartUsage(int id, [FromBody] AddMaintenancePartUsageDto dto, CancellationToken ct)
    {
        var result = await _workflowService.AddPartUsageAsync(id, dto, ct);
        return Ok(result);
    }

    [HttpDelete("{id:int}/parts/{partUsageId:int}")]
    [Authorize(Policy = DataSeeder.PermissionCodes.MaintenanceManage)]
    public async Task<IActionResult> RemovePartUsage(int id, int partUsageId, CancellationToken ct)
    {
        var result = await _workflowService.RemovePartUsageAsync(id, partUsageId, ct);
        return Ok(result);
    }

    [HttpPost("{id:int}/labor-cost")]
    [Authorize(Policy = DataSeeder.PermissionCodes.MaintenanceManage)]
    public async Task<IActionResult> SetLaborCost(int id, [FromBody] SetLaborCostDto dto, CancellationToken ct)
    {
        var result = await _workflowService.SetLaborCostAsync(id, dto.LaborCost, ct);
        return Ok(result);
    }

    /// <summary>GET /api/maintenance-workflow/technicians — options for the assignment picker.</summary>
    [HttpGet("technicians")]
    [Authorize(Policy = DataSeeder.PermissionCodes.MaintenanceView)]
    public async Task<IActionResult> GetTechnicians(CancellationToken ct)
    {
        var result = await _workflowService.GetTechniciansAsync(ct);
        return Ok(result);
    }

    /// <summary>GET /api/maintenance-workflow/{id}/receipt — feeds the A5 print engine on the frontend.</summary>
    [HttpGet("{id:int}/receipt")]
    [Authorize(Policy = DataSeeder.PermissionCodes.MaintenanceView)]
    public async Task<IActionResult> GetReceipt(int id, CancellationToken ct)
    {
        var result = await _workflowService.GetReceiptAsync(id, ct);
        return Ok(result);
    }
}
