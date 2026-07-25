using IbnAlZumar.API.DTOs.Inventory;
using IbnAlZumar.API.Services.Inventory;
using Microsoft.AspNetCore.Mvc;

namespace IbnAlZumar.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
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
    }
}
