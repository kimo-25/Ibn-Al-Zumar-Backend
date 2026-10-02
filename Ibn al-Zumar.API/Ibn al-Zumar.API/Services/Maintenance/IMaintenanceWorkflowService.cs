using IbnAlZumar.API.DTOs.Common;
using IbnAlZumar.API.DTOs.Maintenance;
using IbnAlZumar.Domain.Enums;

namespace IbnAlZumar.API.Services.Maintenance;

public interface IMaintenanceWorkflowService
{
    Task<MaintenanceRequestDetailDto> GetByIdAsync(int id, CancellationToken ct = default);

    Task<PagedResultDto<MaintenanceRequestListItemDto>> GetListAsync(
        MaintenanceStatus? status,
        int? assignedTechnicianUserId,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default);

    Task<MaintenanceRequestDetailDto> AssignTechnicianAsync(int id, int technicianUserId, int actingUserId, CancellationToken ct = default);

    /// <summary>
    /// Validates the transition against the fixed state machine (see MaintenanceWorkflowService's
    /// AllowedTransitions table) before applying it — an invalid transition throws
    /// BadRequestException rather than silently succeeding.
    /// </summary>
    Task<MaintenanceRequestDetailDto> ChangeStatusAsync(int id, MaintenanceStatus newStatus, int actingUserId, string? note, CancellationToken ct = default);

    Task<MaintenanceRequestDetailDto> AddNoteAsync(int id, int actingUserId, string note, CancellationToken ct = default);

    /// <summary>
    /// Decrements ProductStock for the given warehouse and writes a matching InventoryTransaction
    /// in the same DB transaction as the MaintenancePartUsage row (§3.2 invariant).
    /// </summary>
    Task<MaintenanceRequestDetailDto> AddPartUsageAsync(int id, AddMaintenancePartUsageDto dto, CancellationToken ct = default);

    Task<MaintenanceRequestDetailDto> RemovePartUsageAsync(int id, int partUsageId, CancellationToken ct = default);

    Task<MaintenanceRequestDetailDto> SetLaborCostAsync(int id, decimal laborCost, CancellationToken ct = default);

    Task<MaintenanceReceiptDto> GetReceiptAsync(int id, CancellationToken ct = default);

    /// <summary>Lightweight list of active staff for the technician-assignment picker.</summary>
    Task<List<TechnicianOptionDto>> GetTechniciansAsync(CancellationToken ct = default);
}
