using IbnAlZumar.API.DTOs.Purchasing;
using IbnAlZumar.API.Services.Purchasing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IbnAlZumar.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Owner, Admin")]
    public class PurchasingController : ControllerBase
    {
        private readonly IPurchasingService _purchasingService;

        public PurchasingController(IPurchasingService purchasingService)
        {
            _purchasingService = purchasingService;
        }

        // ---------------- Suppliers ----------------

        [HttpGet("suppliers")]
        public async Task<ActionResult<List<SupplierResponseDto>>> GetSuppliers()
        {
            var result = await _purchasingService.GetSuppliersAsync();
            return Ok(result);
        }

        [HttpGet("suppliers/{id:int}")]
        public async Task<ActionResult<SupplierResponseDto>> GetSupplier(int id)
        {
            try
            {
                var result = await _purchasingService.GetSupplierByIdAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("suppliers")]
        public async Task<ActionResult<SupplierResponseDto>> CreateSupplier([FromBody] CreateSupplierDto dto)
        {
            var result = await _purchasingService.CreateSupplierAsync(dto);
            return CreatedAtAction(nameof(GetSupplier), new { id = result.Id }, result);
        }

        [HttpPut("suppliers/{id:int}")]
        public async Task<ActionResult<SupplierResponseDto>> UpdateSupplier(int id, [FromBody] UpdateSupplierDto dto)
        {
            try
            {
                var result = await _purchasingService.UpdateSupplierAsync(id, dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("suppliers/{id:int}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            try
            {
                await _purchasingService.DeleteSupplierAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // ---------------- Purchase Orders ----------------

        [HttpGet("orders")]
        public async Task<ActionResult<List<PurchaseOrderResponseDto>>> GetPurchaseOrders()
        {
            var result = await _purchasingService.GetPurchaseOrdersAsync();
            return Ok(result);
        }

        [HttpGet("orders/{id:int}")]
        public async Task<ActionResult<PurchaseOrderResponseDto>> GetPurchaseOrder(int id)
        {
            try
            {
                var result = await _purchasingService.GetPurchaseOrderByIdAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("orders")]
        public async Task<ActionResult<PurchaseOrderResponseDto>> CreatePurchaseOrder([FromBody] CreatePurchaseOrderDto dto)
        {
            try
            {
                var result = await _purchasingService.CreatePurchaseOrderAsync(dto);
                return CreatedAtAction(nameof(GetPurchaseOrder), new { id = result.Id }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("orders/receive")]
        public async Task<ActionResult<PurchaseOrderResponseDto>> ReceivePurchaseOrder([FromBody] ApprovePurchaseOrderDto dto)
        {
            try
            {
                var result = await _purchasingService.ReceivePurchaseOrderAsync(dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // ---------------- Supplier Accounting (Ledger & Payments) ----------------

        [HttpPost("suppliers/{id:int}/payments")]
        public async Task<ActionResult<SupplierPaymentResponseDto>> CreateSupplierPayment(int id, [FromBody] CreateSupplierPaymentDto dto)
        {
            if (id != dto.SupplierId)
                return BadRequest(new { message = "رقم المورد في المسار لا يطابق رقم المورد في الطلب" });

            try
            {
                var result = await _purchasingService.CreateSupplierPaymentAsync(dto);
                return CreatedAtAction(nameof(GetSupplierLedger), new { id = result.SupplierId }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("suppliers/{id:int}/ledger")]
        public async Task<ActionResult<List<SupplierLedgerEntryDto>>> GetSupplierLedger(int id)
        {
            try
            {
                var result = await _purchasingService.GetSupplierLedgerAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("suppliers/{id:int}/details")]
        public async Task<ActionResult<SupplierDetailsDto>> GetSupplierDetails(int id)
        {
            try
            {
                var result = await _purchasingService.GetSupplierDetailsAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}