using System.Security.Claims;
using IbnAlZumar.API.DTOs.Sync;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.API.Services.Catalog;
using IbnAlZumar.Domain.Entities.Sales;
using IbnAlZumar.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IbnAlZumar.API.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize] // must be a logged-in cashier/employee — never AllowAnonymous for a sync endpoint
public class SyncController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SyncController> _logger;
    private readonly IPricingService _pricingService;

    private const int SqlServerUniqueIndexErrorNumber = 2601;
    private const int SqlServerUniqueConstraintErrorNumber = 2627;

    public SyncController(ApplicationDbContext context, ILogger<SyncController> logger, IPricingService pricingService)
    {
        _context = context;
        _logger = logger;
        _pricingService = pricingService;
    }

    /// <summary>
    /// Receives a batch of offline-created orders from the PWA client and persists
    /// whichever ones are valid, reporting per-item success/failure so the client
    /// can prune its local queue and retry only the failures.
    /// </summary>
    [HttpPost("sync")]
    [ProducesResponseType(typeof(SyncBatchResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Sync([FromBody] SyncBatchRequestDto batch)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var cashierUserId = GetUserIdFromClaims();
        if (cashierUserId is null)
            return Unauthorized(new { message = "لم يتم التعرف على المستخدم." });

        var response = new SyncBatchResponseDto { TotalReceived = batch.Orders.Count };

        // One connection/transaction for the whole batch, with a savepoint per order.
        // This avoids paying the cost of a full BEGIN/COMMIT per row while still letting
        // a single bad order roll back in isolation, without discarding the rest of the batch.
        var strategy = _context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            foreach (var orderDto in batch.Orders)
            {
                var result = await ProcessSingleOrderAsync(orderDto, cashierUserId.Value, transaction);
                response.Results.Add(result);
            }

            await transaction.CommitAsync();
        });

        response.SucceededCount = response.Results.Count(r => r.Success);
        response.FailedCount = response.Results.Count(r => !r.Success);

        return Ok(response);
    }

    private async Task<SyncResultDto> ProcessSingleOrderAsync(
        SyncOrderDto dto,
        int cashierUserId,
        Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction)
    {
        var savepoint = $"sp_{dto.ClientUuid.Replace('-', '_')}";

        try
        {
            await transaction.CreateSavepointAsync(savepoint);

            // --- Idempotency check ---
            var existing = await _context.Orders
                .IgnoreQueryFilters() // include soft-deleted, in case one was ever removed post-sync
                .FirstOrDefaultAsync(o => o.ClientUuid == dto.ClientUuid);

            if (existing != null)
            {
                // Already synced in a previous attempt — report success without re-inserting.
                return new SyncResultDto
                {
                    ClientUuid = dto.ClientUuid,
                    Success = true,
                    ServerId = existing.Id,
                };
            }

            // --- Referential validation (fail fast, don't trust client-side IDs) ---
            var warehouseExists = await _context.Warehouses
                .AnyAsync(w => w.Id == dto.WarehouseId && w.IsActive);
            if (!warehouseExists)
            {
                return Failure(dto.ClientUuid, "INVALID_WAREHOUSE", "المخزن المحدد غير موجود أو غير نشط.");
            }

            var productIds = dto.Items.Select(i => i.ProductId).Distinct().ToList();
            var validProductIds = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync();

            var missingProducts = productIds.Except(validProductIds).ToList();
            if (missingProducts.Count > 0)
            {
                return Failure(dto.ClientUuid, "INVALID_PRODUCT",
                    $"منتجات غير موجودة: {string.Join(", ", missingProducts)}");
            }

            if (dto.CustomerId.HasValue)
            {
                var customerExists = await _context.Customers.AnyAsync(c => c.Id == dto.CustomerId.Value);
                if (!customerExists)
                    return Failure(dto.ClientUuid, "INVALID_CUSTOMER", "العميل المحدد غير موجود.");
            }

            // --- Build the order, recomputing money server-side rather than trusting client totals ---
            var items = new List<OrderItem>();
            decimal subTotal = 0;

            foreach (var itemDto in dto.Items)
            {
                // Resolve authoritative unit price using the order-level tier provided by the POS.
                var authoritativeUnitPrice = await _pricingService.ResolveUnitPriceAsync(
                    itemDto.ProductId,
                    itemDto.ProductVariantId,
                    dto.PricingTier,
                    itemDto.Quantity);

                var grossLine = authoritativeUnitPrice * itemDto.Quantity;
                var lineDiscount = CalculateDiscount(grossLine, itemDto.DiscountType, itemDto.DiscountValue);
                var lineTotal = grossLine - lineDiscount;

                items.Add(new OrderItem
                {
                    ProductId = itemDto.ProductId,
                    ProductVariantId = itemDto.ProductVariantId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = authoritativeUnitPrice,
                    DiscountType = itemDto.DiscountType,
                    DiscountValue = itemDto.DiscountValue,
                    DiscountAmount = lineDiscount,
                    LineTotal = lineTotal,
                });

                subTotal += grossLine;
            }

            var orderDiscount = CalculateDiscount(subTotal, dto.DiscountType, dto.DiscountValue);

            // Use same OrderNumber sequence/format as online OrderService.GenerateOrderNumberAsync
            var sequenceValue = await _context.Database
                .SqlQueryRaw<long>("SELECT NEXT VALUE FOR dbo.OrderNumberSeq AS [Value]")
                .SingleAsync();
            var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{sequenceValue:D6}";

            var order = new Order
            {
                ClientUuid = dto.ClientUuid,
                OrderNumber = orderNumber,
                CustomerId = dto.CustomerId,
                GuestName = dto.GuestName,
                GuestPhone = dto.GuestPhone,
                Source = dto.Source,
                Status = OrderStatus.PendingConfirmation,
                PaymentMethod = dto.PaymentMethod,
                WarehouseId = dto.WarehouseId,
                CashierUserId = cashierUserId, // taken from the authenticated token, never from the client body
                OrderDate = dto.OrderDate,
                ShippingAddress = dto.ShippingAddress,
                DiscountType = dto.DiscountType,
                DiscountValue = dto.DiscountValue,
                DiscountAmount = orderDiscount,
                SubTotal = subTotal,
                TotalAmount = subTotal - orderDiscount,
                Notes = dto.Notes,
                Items = items,
                // Persist the pricing tier used by this offline sale (order-level).
                PricingTier = dto.PricingTier,
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return new SyncResultDto
            {
                ClientUuid = dto.ClientUuid,
                Success = true,
                ServerId = order.Id,
            };
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            // Lost a race with another concurrent sync request for the same ClientUuid.
            // Treat as success — the record exists, which is what the client wants.
            await transaction.RollbackToSavepointAsync(savepoint);

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.ClientUuid == dto.ClientUuid);
            return new SyncResultDto { ClientUuid = dto.ClientUuid, Success = true, ServerId = order?.Id };
        }
        catch (Exception ex)
        {
            await transaction.RollbackToSavepointAsync(savepoint);
            _logger.LogError(ex, "Sync failed for order {ClientUuid}", dto.ClientUuid);
            return Failure(dto.ClientUuid, "SERVER_ERROR", "حدث خطأ أثناء معالجة الطلب.");
        }
    }

    private static decimal CalculateDiscount(decimal baseAmount, DiscountType type, decimal value) =>
        type switch
        {
            DiscountType.Percentage => Math.Round(baseAmount * (value / 100m), 2),
            DiscountType.None => 0m,
            _ => Math.Min(value, baseAmount),
        };

    private static SyncResultDto Failure(string clientUuid, string code, string message) => new()
    {
        ClientUuid = clientUuid,
        Success = false,
        ErrorCode = code,
        ErrorMessage = message,
    };

    private static bool IsUniqueViolation(DbUpdateException ex)
    {
        return ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx
            && (sqlEx.Number == SqlServerUniqueIndexErrorNumber
                || sqlEx.Number == SqlServerUniqueConstraintErrorNumber);
    }

    private int? GetUserIdFromClaims()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(idClaim, out var id) ? id : null;
    }
}