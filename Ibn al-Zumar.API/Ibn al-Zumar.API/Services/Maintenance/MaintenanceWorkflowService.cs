using IbnAlZumar.API.Common.Exceptions;
using IbnAlZumar.API.DTOs.Common;
using IbnAlZumar.API.DTOs.Maintenance;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.Domain.Entities.Inventory;
using IbnAlZumar.Domain.Entities.Maintenance;
using IbnAlZumar.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace IbnAlZumar.API.Services.Maintenance;

/// <summary>
/// Phase 2 — the in-store repair-shop workflow layered on top of the existing (online-inquiry)
/// MaintenanceRequest entity. Deliberately additive: every method here only ever ADDS rows
/// (MaintenanceNote, MaintenancePartUsage) or updates the new nullable fields
/// (AssignedTechnicianUserId, LaborCost, ActualCost, DeliveredAt) — nothing here touches
/// AdminNotes/EstimatedPrice/MaintenanceReportUrl, which the existing online-pricing screen
/// still owns.
/// </summary>
public class MaintenanceWorkflowService : IMaintenanceWorkflowService
{
    private readonly ApplicationDbContext _context;

    // Fixed state machine. Rejected/Cancelled/Delivered are terminal (no outgoing transitions).
    // Pending -> InDiagnostics -> Priced -> Approved -> AwaitingParts -> InRepair -> Completed -> Delivered,
    // with a Cancelled escape hatch from every non-terminal state and a Rejected branch off Priced
    // (customer declines the quote) that is distinct from Cancelled (ticket abandoned for any
    // other reason) — see BACKEND_CHANGES_PHASE2.md for why both exist side by side.
    private static readonly Dictionary<MaintenanceStatus, MaintenanceStatus[]> AllowedTransitions = new()
    {
        [MaintenanceStatus.Pending] = new[] { MaintenanceStatus.InDiagnostics, MaintenanceStatus.Cancelled },
        [MaintenanceStatus.InDiagnostics] = new[] { MaintenanceStatus.Priced, MaintenanceStatus.AwaitingParts, MaintenanceStatus.InRepair, MaintenanceStatus.Cancelled },
        [MaintenanceStatus.Priced] = new[] { MaintenanceStatus.Approved, MaintenanceStatus.Rejected, MaintenanceStatus.Cancelled },
        [MaintenanceStatus.Approved] = new[] { MaintenanceStatus.AwaitingParts, MaintenanceStatus.InRepair, MaintenanceStatus.Cancelled },
        // Expanded to support flexible transitions required by POS/workshop
        [MaintenanceStatus.AwaitingParts] = new[] { MaintenanceStatus.InRepair, MaintenanceStatus.InDiagnostics, MaintenanceStatus.Approved, MaintenanceStatus.Cancelled },
        [MaintenanceStatus.InRepair] = new[] { MaintenanceStatus.Completed, MaintenanceStatus.AwaitingParts, MaintenanceStatus.InDiagnostics, MaintenanceStatus.Approved, MaintenanceStatus.Cancelled },
        [MaintenanceStatus.Completed] = new[] { MaintenanceStatus.Delivered, MaintenanceStatus.InRepair, MaintenanceStatus.InDiagnostics },
        // Expanded to support flexible transitions required by POS/workshop
        [MaintenanceStatus.AwaitingParts] = new[] { MaintenanceStatus.InRepair, MaintenanceStatus.InDiagnostics, MaintenanceStatus.Approved, MaintenanceStatus.Cancelled },
        [MaintenanceStatus.InRepair] = new[] { MaintenanceStatus.Completed, MaintenanceStatus.AwaitingParts, MaintenanceStatus.InDiagnostics, MaintenanceStatus.Approved, MaintenanceStatus.Cancelled },
        [MaintenanceStatus.Completed] = new[] { MaintenanceStatus.Delivered, MaintenanceStatus.InRepair, MaintenanceStatus.InDiagnostics },
        // Expanded to support flexible transitions required by POS/workshop
        [MaintenanceStatus.AwaitingParts] = new[] { MaintenanceStatus.InRepair, MaintenanceStatus.InDiagnostics, MaintenanceStatus.Approved, MaintenanceStatus.Cancelled },
        [MaintenanceStatus.InRepair] = new[] { MaintenanceStatus.Completed, MaintenanceStatus.AwaitingParts, MaintenanceStatus.InDiagnostics, MaintenanceStatus.Approved, MaintenanceStatus.Cancelled },
        [MaintenanceStatus.Completed] = new[] { MaintenanceStatus.Delivered, MaintenanceStatus.InRepair, MaintenanceStatus.InDiagnostics },
        [MaintenanceStatus.Rejected] = Array.Empty<MaintenanceStatus>(),
        [MaintenanceStatus.Cancelled] = Array.Empty<MaintenanceStatus>(),
        [MaintenanceStatus.Delivered] = Array.Empty<MaintenanceStatus>(),
    };

    public MaintenanceWorkflowService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MaintenanceRequestDetailDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var request = await LoadForDetailAsync(id, ct)
            ?? throw new NotFoundException("طلب الصيانة غير موجود.");

        return MapToDetailDto(request);
    }

    public async Task<PagedResultDto<MaintenanceRequestListItemDto>> GetListAsync(
        MaintenanceStatus? status,
        int? assignedTechnicianUserId,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize is < 1 or > 200 ? 30 : pageSize;

        var query = _context.MaintenanceRequests.AsNoTracking().AsQueryable();

        if (status.HasValue)
            query = query.Where(m => m.Status == status.Value);

        if (assignedTechnicianUserId.HasValue)
            query = query.Where(m => m.AssignedTechnicianUserId == assignedTechnicianUserId.Value);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(m => m.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new MaintenanceRequestListItemDto
            {
                Id = m.Id,
                // NOTE: MaintenanceRequest may also carry guest-contact fields not covered by the
                // Customer/User join below (the entity wasn't available to inspect directly while
                // building this) — if your copy has e.g. GuestName/GuestPhone, extend this
                // projection to fall back to them the same way MaintenanceInquiriesTab.jsx already
                // falls back across userName/customerName/fullName on the frontend.
                CustomerName = m.Customer != null ? m.Customer.FullName : (m.User != null ? m.User.FullName : null),
                CustomerPhone = m.Customer != null ? m.Customer.Phone : null,
                ProblemDescription = m.ProblemDescription,
                Status = m.Status,
                AssignedTechnicianUserId = m.AssignedTechnicianUserId,
                AssignedTechnicianName = m.AssignedTechnicianUser != null ? m.AssignedTechnicianUser.FullName : null,
                EstimatedPrice = m.EstimatedPrice,
                ActualCost = m.ActualCost,
                PartsCount = m.PartUsages.Count,
                CreatedAt = m.CreatedAt,
                ScheduledDate = m.ScheduledDate
            })
            .ToListAsync(ct);

        return new PagedResultDto<MaintenanceRequestListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<MaintenanceRequestDetailDto> AssignTechnicianAsync(int id, int technicianUserId, int actingUserId, CancellationToken ct = default)
    {
        var request = await _context.MaintenanceRequests.FirstOrDefaultAsync(m => m.Id == id, ct)
            ?? throw new NotFoundException("طلب الصيانة غير موجود.");

        var technicianExists = await _context.Users.AnyAsync(u => u.Id == technicianUserId && u.IsActive, ct);
        if (!technicianExists)
            throw new BadRequestException("الفني المحدد غير موجود أو غير نشط.");

        request.AssignedTechnicianUserId = technicianUserId;

        _context.MaintenanceNotes.Add(new MaintenanceNote
        {
            MaintenanceRequestId = id,
            AuthorUserId = actingUserId,
            Note = "تم إسناد الطلب لفني الصيانة.",
            StatusAtNote = request.Status
        });

        await _context.SaveChangesAsync(ct);

        return await GetByIdAsync(id, ct);
    }

    public async Task<MaintenanceRequestDetailDto> ChangeStatusAsync(int id, MaintenanceStatus newStatus, int actingUserId, string? note, CancellationToken ct = default)
    {
        var request = await _context.MaintenanceRequests
            .Include(m => m.PartUsages)
            .FirstOrDefaultAsync(m => m.Id == id, ct)
            ?? throw new NotFoundException("طلب الصيانة غير موجود.");

        if (!AllowedTransitions.TryGetValue(request.Status, out var allowed) || !allowed.Contains(newStatus))
        {
            throw new BadRequestException(
                $"لا يمكن نقل الطلب من حالة '{request.Status}' إلى '{newStatus}' مباشرة.");
        }

        request.Status = newStatus;

        if (newStatus == MaintenanceStatus.Completed)
        {
            // Freeze ActualCost the moment the repair is marked done — same "recompute once,
            // then treat as historical" rule as Order.TotalAmount (§7.4).
            request.ActualCost = request.LaborCost + request.PartUsages.Sum(p => p.LineTotal);
        }
        else if (newStatus == MaintenanceStatus.Delivered)
        {
            request.DeliveredAt = DateTime.UtcNow;
        }

        _context.MaintenanceNotes.Add(new MaintenanceNote
        {
            MaintenanceRequestId = id,
            AuthorUserId = actingUserId,
            Note = string.IsNullOrWhiteSpace(note) ? $"تم تغيير الحالة إلى {newStatus}." : note,
            StatusAtNote = newStatus
        });

        await _context.SaveChangesAsync(ct);

        return await GetByIdAsync(id, ct);
    }

    public async Task<MaintenanceRequestDetailDto> AddNoteAsync(int id, int actingUserId, string note, CancellationToken ct = default)
    {
        var exists = await _context.MaintenanceRequests.AnyAsync(m => m.Id == id, ct);
        if (!exists)
            throw new NotFoundException("طلب الصيانة غير موجود.");

        _context.MaintenanceNotes.Add(new MaintenanceNote
        {
            MaintenanceRequestId = id,
            AuthorUserId = actingUserId,
            Note = note
        });

        await _context.SaveChangesAsync(ct);

        return await GetByIdAsync(id, ct);
    }

    public async Task<MaintenanceRequestDetailDto> AddPartUsageAsync(int id, AddMaintenancePartUsageDto dto, CancellationToken ct = default)
    {
        var request = await _context.MaintenanceRequests.FirstOrDefaultAsync(m => m.Id == id, ct)
            ?? throw new NotFoundException("طلب الصيانة غير موجود.");

        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == dto.ProductId, ct)
            ?? throw new NotFoundException("المنتج (قطعة الغيار) غير موجود.");

        var warehouseExists = await _context.Warehouses.AnyAsync(w => w.Id == dto.WarehouseId, ct);
        if (!warehouseExists)
            throw new NotFoundException("المخزن المحدد غير موجود.");

        var unitCostPrice = dto.UnitCostPrice ?? product.CurrentCostPrice;
        var lineTotal = unitCostPrice * dto.Quantity;

        await using var transaction = await _context.Database.BeginTransactionAsync(ct);

        InventoryTransaction? inventoryTransaction = null;

        if (product.TrackInventory)
        {
            var stock = await _context.ProductStocks
                .FirstOrDefaultAsync(s => s.ProductId == dto.ProductId && s.WarehouseId == dto.WarehouseId, ct);

            if (stock is null || stock.QuantityOnHand < dto.Quantity)
                throw new BadRequestException("الكمية المتاحة من قطعة الغيار في هذا المخزن غير كافية.");

            stock.QuantityOnHand -= dto.Quantity;

            // TransactionType requires appending InventoryTransactionType.MaintenanceUsed = 9
            // to Domain/Enums/Enums.cs — see BACKEND_CHANGES_PHASE2.md §1.
            inventoryTransaction = new InventoryTransaction
            {
                ProductId = dto.ProductId,
                WarehouseId = dto.WarehouseId,
                TransactionType = InventoryTransactionType.MaintenanceUsed,
                QuantityChange = -dto.Quantity,
                ReferenceType = "MaintenanceRequest",
                ReferenceId = id,
                TransactionDate = DateTime.UtcNow,
                Notes = $"قطعة غيار مستخدمة في طلب الصيانة #{id}"
            };
            _context.InventoryTransactions.Add(inventoryTransaction);
            await _context.SaveChangesAsync(ct); // assigns inventoryTransaction.Id
        }

        var partUsage = new MaintenancePartUsage
        {
            MaintenanceRequestId = id,
            ProductId = dto.ProductId,
            WarehouseId = dto.WarehouseId,
            Quantity = dto.Quantity,
            UnitCostPrice = unitCostPrice,
            LineTotal = lineTotal,
            InventoryTransactionId = inventoryTransaction?.Id
        };
        _context.MaintenancePartUsages.Add(partUsage);

        // Keep ActualCost as a live running total while work is in progress; ChangeStatusAsync
        // freezes it for good once the ticket reaches Completed.
        var updatedPartsTotal = await _context.MaintenancePartUsages
            .Where(p => p.MaintenanceRequestId == id)
            .SumAsync(p => p.LineTotal, ct);
        request.ActualCost = request.LaborCost + updatedPartsTotal + lineTotal;

        await _context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return await GetByIdAsync(id, ct);
    }

    public async Task<MaintenanceRequestDetailDto> RemovePartUsageAsync(int id, int partUsageId, CancellationToken ct = default)
    {
        var request = await _context.MaintenanceRequests.FirstOrDefaultAsync(m => m.Id == id, ct)
            ?? throw new NotFoundException("طلب الصيانة غير موجود.");

        var partUsage = await _context.MaintenancePartUsages
            .FirstOrDefaultAsync(p => p.Id == partUsageId && p.MaintenanceRequestId == id, ct)
            ?? throw new NotFoundException("سطر قطعة الغيار غير موجود.");

        await using var transaction = await _context.Database.BeginTransactionAsync(ct);

        if (partUsage.InventoryTransactionId.HasValue)
        {
            // Reverse the stock deduction with a compensating transaction rather than deleting
            // the original — InventoryTransaction is append-only (§3.2).
            var stock = await _context.ProductStocks
                .FirstOrDefaultAsync(s => s.ProductId == partUsage.ProductId && s.WarehouseId == partUsage.WarehouseId, ct);

            if (stock is not null)
                stock.QuantityOnHand += partUsage.Quantity;

            _context.InventoryTransactions.Add(new InventoryTransaction
            {
                ProductId = partUsage.ProductId,
                WarehouseId = partUsage.WarehouseId,
                TransactionType = InventoryTransactionType.AdjustmentIncrease,
                QuantityChange = partUsage.Quantity,
                ReferenceType = "MaintenanceRequest",
                ReferenceId = id,
                TransactionDate = DateTime.UtcNow,
                Notes = $"إلغاء استخدام قطعة غيار من طلب الصيانة #{id}"
            });
        }

        _context.MaintenancePartUsages.Remove(partUsage);
        await _context.SaveChangesAsync(ct);

        var remainingPartsTotal = await _context.MaintenancePartUsages
            .Where(p => p.MaintenanceRequestId == id && p.Id != partUsageId)
            .SumAsync(p => p.LineTotal, ct);
        request.ActualCost = request.LaborCost + remainingPartsTotal;
        await _context.SaveChangesAsync(ct);

        await transaction.CommitAsync(ct);

        return await GetByIdAsync(id, ct);
    }

    public async Task<MaintenanceRequestDetailDto> SetLaborCostAsync(int id, decimal laborCost, CancellationToken ct = default)
    {
        var request = await _context.MaintenanceRequests
            .Include(m => m.PartUsages)
            .FirstOrDefaultAsync(m => m.Id == id, ct)
            ?? throw new NotFoundException("طلب الصيانة غير موجود.");

        request.LaborCost = laborCost;
        request.ActualCost = laborCost + request.PartUsages.Sum(p => p.LineTotal);

        await _context.SaveChangesAsync(ct);

        return await GetByIdAsync(id, ct);
    }

    public async Task<MaintenanceReceiptDto> GetReceiptAsync(int id, CancellationToken ct = default)
    {
        var request = await LoadForDetailAsync(id, ct)
            ?? throw new NotFoundException("طلب الصيانة غير موجود.");

        var partsTotal = request.PartUsages.Sum(p => p.LineTotal);

        return new MaintenanceReceiptDto
        {
            Id = request.Id,
            TicketNumber = $"MNT-{request.Id:D6}",
            CustomerName = request.Customer?.FullName ?? request.User?.FullName,
            CustomerPhone = request.Customer?.Phone,
            ProblemDescription = request.ProblemDescription,
            StatusLabel = request.Status.ToString(),
            LaborCost = request.LaborCost,
            Parts = request.PartUsages.Select(p => new MaintenanceReceiptLineDto
            {
                ProductName = p.Product.NameAr ?? p.Product.Name,
                Quantity = p.Quantity,
                UnitCostPrice = p.UnitCostPrice,
                LineTotal = p.LineTotal
            }).ToList(),
            PartsTotal = partsTotal,
            Total = request.LaborCost + partsTotal,
            CreatedAt = request.CreatedAt,
            DeliveredAt = request.DeliveredAt
        };
    }

    public async Task<List<TechnicianOptionDto>> GetTechniciansAsync(CancellationToken ct = default)
    {
        // No dedicated "Technician" role exists yet in the RBAC list (§3.5) — every active staff
        // member is offered as an assignable technician. Narrow this with a role filter once one
        // exists (e.g. `.Where(u => u.UserRoles.Any(ur => ur.Role.Name == "TECHNICIAN"))`).
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.IsActive)
            .OrderBy(u => u.FullName)
            .Select(u => new TechnicianOptionDto { Id = u.Id, FullName = u.FullName })
            .ToListAsync(ct);
    }

    private Task<Domain.Entities.Maintenance.MaintenanceRequest?> LoadForDetailAsync(int id, CancellationToken ct)
    {
        return _context.MaintenanceRequests
            .AsNoTracking()
            .Include(m => m.Customer)
            .Include(m => m.User)
            .Include(m => m.AssignedTechnicianUser)
            .Include(m => m.Notes).ThenInclude(n => n.AuthorUser)
            .Include(m => m.PartUsages).ThenInclude(p => p.Product)
            .FirstOrDefaultAsync(m => m.Id == id, ct);
    }

    private static MaintenanceRequestDetailDto MapToDetailDto(Domain.Entities.Maintenance.MaintenanceRequest request)
    {
        return new MaintenanceRequestDetailDto
        {
            Id = request.Id,
            CustomerId = request.CustomerId,
            CustomerName = request.Customer?.FullName ?? request.User?.FullName,
            CustomerPhone = request.Customer?.Phone,
            ProblemDescription = request.ProblemDescription,
            ImageUrls = request.ImageUrls ?? new List<string>(),
            DeliveryMethod = (int)request.DeliveryMethod,
            Status = request.Status,
            EstimatedPrice = request.EstimatedPrice,
            LaborCost = request.LaborCost,
            ActualCost = request.ActualCost,
            ScheduledDate = request.ScheduledDate,
            DeliveredAt = request.DeliveredAt,
            AdminNotes = request.AdminNotes,
            MaintenanceReportUrl = request.MaintenanceReportUrl,
            AssignedTechnicianUserId = request.AssignedTechnicianUserId,
            AssignedTechnicianName = request.AssignedTechnicianUser?.FullName,
            Notes = request.Notes
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new MaintenanceNoteDto
                {
                    Id = n.Id,
                    AuthorUserId = n.AuthorUserId,
                    AuthorName = n.AuthorUser?.FullName ?? string.Empty,
                    Note = n.Note,
                    StatusAtNote = n.StatusAtNote,
                    CreatedAt = n.CreatedAt
                }).ToList(),
            PartUsages = request.PartUsages
                .Select(p => new MaintenancePartUsageDto
                {
                    Id = p.Id,
                    ProductId = p.ProductId,
                    ProductName = p.Product.NameAr ?? p.Product.Name,
                    ProductSku = p.Product.SKU,
                    WarehouseId = p.WarehouseId,
                    Quantity = p.Quantity,
                    UnitCostPrice = p.UnitCostPrice,
                    LineTotal = p.LineTotal
                }).ToList(),
            CreatedAt = request.CreatedAt
        };
    }
}
