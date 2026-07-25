using IbnAlZumar.API.DTOs.Purchasing;
using IbnAlZumar.API.Services.Purchasing;
using Microsoft.AspNetCore.Mvc;

namespace IbnAlZumar.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchasingController : ControllerBase
    {
        private readonly IPurchasingService _purchasingService;

        public PurchasingController(IPurchasingService purchasingService)
        {
            _purchasingService = purchasingService;
        }

        [HttpPost("suppliers")]
        public async Task<ActionResult<SupplierResponseDto>> CreateSupplier([FromBody] CreateSupplierDto dto)
        {
            var result = await _purchasingService.CreateSupplierAsync(dto);
            return Ok(result);
        }

        [HttpPost("orders")]
        public async Task<ActionResult<PurchaseOrderResponseDto>> CreatePurchaseOrder([FromBody] CreatePurchaseOrderDto dto)
        {
            var result = await _purchasingService.CreatePurchaseOrderAsync(dto);
            return Ok(result);
        }
    }
}