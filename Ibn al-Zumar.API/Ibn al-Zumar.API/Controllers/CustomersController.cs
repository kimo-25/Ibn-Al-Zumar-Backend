using IbnAlZumar.API.Common.Exceptions;
using IbnAlZumar.API.DTOs.Customers;
using IbnAlZumar.API.Services.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IbnAlZumar.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        [Authorize(Roles = "Owner,STORE_OWNER,Admin,SuperAdmin,Moderator,Cashier,Store POS")]
        public async Task<IActionResult> GetAll([FromQuery] CustomerFilterDto filter)
        {
            var result = await _customerService.GetAllAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Owner,STORE_OWNER,Admin,SuperAdmin,Moderator,Cashier,Store POS")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var c = await _customerService.GetByIdAsync(id);
                return Ok(c);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Owner,STORE_OWNER,Admin,SuperAdmin,Moderator,Cashier,Store POS")]
        public async Task<IActionResult> Create([FromBody] CreateCustomerDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var created = await _customerService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Owner,STORE_OWNER,Admin,SuperAdmin,Moderator")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSalesCustomerDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var updated = await _customerService.UpdateAsync(id, dto);
                return Ok(updated);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Owner,STORE_OWNER,Admin,SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _customerService.DeleteAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id:int}/adjust-debt")]
        [Authorize(Roles = "Owner,STORE_OWNER,Admin,SuperAdmin")]
        public async Task<IActionResult> AdjustDebt(int id, [FromBody] AdjustCustomerDebtDto dto)
        {
            try
            {
                var updated = await _customerService.AdjustDebtAsync(id, dto);
                return Ok(updated);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}