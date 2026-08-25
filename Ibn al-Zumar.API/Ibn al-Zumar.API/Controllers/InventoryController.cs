using IbnAlZumar.API.DTOs.Inventory;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.API.Services.Inventory;
using IbnAlZumar.Domain.Entities.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IbnAlZumar.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly ApplicationDbContext _context;

        public InventoryController(IInventoryService inventoryService, ApplicationDbContext context)
        {
            _inventoryService = inventoryService;
            _context = context;
        }

        [HttpPost("adjust")]
        public async Task<ActionResult<StockTransactionResponseDto>> AdjustStock([FromBody] AdjustStockDto dto)
        {
            var result = await _inventoryService.AdjustStockAsync(dto);
            return Ok(result);
        }

        [HttpPost("transfer")]
        public async Task<ActionResult<StockTransactionResponseDto>> TransferStock([FromBody] TransferStockDto dto)
        {
            var result = await _inventoryService.TransferStockAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// GET /api/inventory/warehouses
        /// Returns all active warehouses for selection in purchase orders and stock operations.
        /// </summary>
        [HttpGet("warehouses")]
        public async Task<ActionResult<IEnumerable<object>>> GetWarehouses()
        {
            var warehouses = await _context.Set<Warehouse>()
                .Where(w => w.IsActive)
                .Select(w => new { w.Id, w.Name, w.Address, w.IsMainWarehouse })
                .ToListAsync();

            return Ok(warehouses);
        }

        /// <summary>
        /// GET /api/inventory/stock-levels?warehouseId=1&search=
        /// يُرجع جميع المنتجات النشطة حتى لو كان رصيدها صفر في المخزن المختار
        /// </summary>
        [HttpGet("stock-levels")]
        public async Task<ActionResult<IEnumerable<object>>> GetStockLevels([FromQuery] int? warehouseId, [FromQuery] string? search)
        {
            var query = _context.Products
                .AsNoTracking()
                .Where(p => p.IsActive);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.Trim().ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(searchLower) ||
                    (p.NameAr != null && p.NameAr.ToLower().Contains(searchLower)) ||
                    p.SKU.ToLower().Contains(searchLower));
            }

            var result = await query
                .Select(p => new
                {
                    productId = p.Id,
                    productName = p.Name,
                    productNameAr = p.NameAr,
                    sku = p.SKU,
                    quantityOnHand = warehouseId.HasValue
                        ? p.Stocks.Where(s => s.WarehouseId == warehouseId.Value).Select(s => (int?)s.QuantityOnHand).FirstOrDefault() ?? 0
                        : p.Stocks.Sum(s => (int?)s.QuantityOnHand) ?? 0
                })
                .Take(150)
                .ToListAsync();

            return Ok(result);
        }

        /// <summary>
        /// GET /api/inventory/transactions?warehouseId=1&take=60
        /// يُرجع سجل حركات المخزون الأخيرة
        /// </summary>
        [HttpGet("transactions")]
        public async Task<ActionResult<IEnumerable<object>>> GetTransactions([FromQuery] int? warehouseId, [FromQuery] int take = 60)
        {
            var query = _context.Set<InventoryTransaction>()
                .AsNoTracking()
                .Include(t => t.Product)
                .Include(t => t.Warehouse)
                .AsQueryable();

            if (warehouseId.HasValue)
            {
                query = query.Where(t => t.WarehouseId == warehouseId.Value);
            }

            var transactions = await query
                .OrderByDescending(t => t.TransactionDate)
                .Take(take)
                .Select(t => new
                {
                    id = t.Id,
                    productName = t.Product.Name,
                    productNameAr = t.Product.NameAr,
                    sku = t.Product.SKU,
                    warehouseName = t.Warehouse.Name,
                    transactionType = t.TransactionType.ToString(),
                    quantityChange = t.QuantityChange,
                    notes = t.Notes,
                    transactionDate = t.TransactionDate
                })
                .ToListAsync();

            return Ok(transactions);
        }

        /// <summary>
        /// GET /api/inventory/low-stock
        /// Returns every tracked, active product whose current stock has dropped to (or below) its MinStockThreshold
        /// </summary>
        [HttpGet("low-stock")]
        [Authorize(Roles = "Owner, Admin, Moderator")]
        [ProducesResponseType(typeof(List<LowStockProductDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<LowStockProductDto>>> GetLowStock()
        {
            var lowStockProducts = await _context.Products
                .AsNoTracking()
                .Where(p => p.TrackInventory && p.IsActive)
                .Select(p => new LowStockProductDto
                {
                    Id = p.Id,
                    SKU = p.SKU,
                    Name = p.Name,
                    NameAr = p.NameAr,
                    CurrentStock = p.Stocks.Sum(s => (int?)s.QuantityOnHand) ?? 0,
                    MinStockThreshold = p.MinStockThreshold,
                    UnitPrice = p.SellingPrice,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    ImageUrl = p.ImageUrl
                })
                .Where(p => p.CurrentStock <= p.MinStockThreshold)
                .OrderBy(p => p.CurrentStock)
                .ToListAsync();

            return Ok(lowStockProducts);
        }
    }
}