using System.Security.Claims;
using IbnAlZumar.API.Common.Exceptions;
using IbnAlZumar.API.DTOs.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Sales;

namespace IbnAlZumar.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// جلب كافة الطلبات للإدارة والموديريتور
        /// </summary>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        /// <summary>
        /// إنشاء طلب جديد من متجر العملاء
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userEmail = GetUserEmailFromClaims();

                if (!string.IsNullOrEmpty(userEmail))
                {
                    // إسناد الإيميل لـ DTO ليتم ربطه بالعميل في الـ Service
                    dto.CustomerEmail = userEmail;
                }

                var order = await _orderService.CreateAsync(dto);
                return StatusCode(StatusCodes.Status201Created, order);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// جلب طلبات العميل الحالي المسجل
        /// </summary>
        [HttpGet("my-orders")]
        [Authorize]
        [ProducesResponseType(typeof(List<CustomerOrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<CustomerOrderDto>>> GetMyOrders()
        {
            var userEmail = GetUserEmailFromClaims();

            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new { message = "لم يتم العثور على البريد الإلكتروني في بيانات الاعتماد." });

            var orders = await _orderService.GetMyOrdersAsync(userEmail);
            return Ok(orders);
        }

        /// <summary>
        /// ترقية حالة الطلب مرحلياً من لوحة العمليات
        /// </summary>
        [HttpPut("{id}/advance-status")]
        [Authorize]
        public async Task<IActionResult> AdvanceOrderStatus(int id)
        {
            await _orderService.AdvanceOrderStatusAsync(id);
            return Ok(new { message = "تم تحديث حالة الطلب بنجاح." });
        }

        private string? GetUserEmailFromClaims()
        {
            return User.FindFirstValue(ClaimTypes.Email)
                ?? User.FindFirstValue(ClaimTypes.Name)
                ?? User.FindFirstValue("email");
        }
    }
}