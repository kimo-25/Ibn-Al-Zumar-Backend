using System.Security.Claims;
using IbnAlZumar.API.Common.Exceptions;
using IbnAlZumar.API.DTOs.Sales;
using IbnAlZumar.Domain.Enums;
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
        /// جلب كافة الطلبات للإدارة والموديريتور فقط
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Moderator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        /// <summary>
        /// جلب تفاصيل طلب معين موحدة للعميل وللأدمن
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(CustomerOrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetOrderDetails(int id)
        {
            try
            {
                var userEmail = GetUserEmailFromClaims();
                var isAdminOrMod = User.IsInRole("Admin") || User.IsInRole("Moderator");

                var orderDto = await _orderService.GetOrderDetailsAsync(id, userEmail, isAdminOrMod);
                return Ok(orderDto);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        /// <summary>
        /// إنشاء طلب جديد من متجر العملاء أو الكاشير (POS)
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userEmail = GetUserEmailFromClaims();

                if (!string.IsNullOrEmpty(userEmail))
                {
                    dto.CustomerEmail = userEmail;
                }

                var order = await _orderService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetOrderDetails), new { id = order.Id }, order);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "حدث خطأ غير متوقع أثناء معالجة الطلب." });
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
        /// ترقية حالة الطلب مرحلياً (للإدارة فقط)
        /// </summary>
        [HttpPut("{id}/advance-status")]
        [Authorize(Roles = "Admin,Moderator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AdvanceOrderStatus(int id)
        {
            try
            {
                await _orderService.AdvanceOrderStatusAsync(id);
                return Ok(new { message = "تم تحديث حالة الطلب بنجاح." });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "حدث خطأ غير متوقع أثناء تحديث الحالة." });
            }
        }

        /// <summary>
        /// تغيير حالة الطلب مباشرة إلى حالة محددة (للإدارة فقط) - تُستخدم لإلغاء الفواتير الخاطئة
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Moderator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromQuery] OrderStatus status)
        {
            try
            {
                await _orderService.UpdateOrderStatusAsync(id, status);
                return Ok(new { message = "تم تغيير حالة الطلب بنجاح." });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "حدث خطأ غير متوقع أثناء تغيير الحالة." });
            }
        }

        // ================= Endpoints الإلغاء الجديدة =================

        /// <summary>
        /// العميل بيطلب إلغاء الأوردر مع إرسال السبب (يُستخدم في الفرونت إند بتاع الكاستمر)
        /// </summary>
        [HttpPost("{id}/request-cancel")]
        [Authorize]
        public async Task<IActionResult> RequestCancel(int id, [FromBody] string reason)
        {
            try
            {
                var userEmail = GetUserEmailFromClaims();
                if (string.IsNullOrEmpty(userEmail)) return Unauthorized(new { message = "غير مصرح." });

                await _orderService.RequestCancelOrderAsync(id, reason, userEmail);
                return Ok(new { message = "تم إرسال طلب الإلغاء للمراجعة وسيتم الرد عليكم." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// المودريتور بيوافق على إلغاء الأوردر (يُستخدم في لوحة التحكم)
        /// </summary>
        [HttpPost("{id}/approve-cancel")]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> ApproveCancel(int id)
        {
            try
            {
                await _orderService.ApproveCancelOrderAsync(id);
                return Ok(new { message = "تم إلغاء الطلب وإرسال إشعار للعميل بنجاح." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private string? GetUserEmailFromClaims()
        {
            return User.FindFirstValue(ClaimTypes.Email)
                ?? User.FindFirstValue(ClaimTypes.Name)
                ?? User.FindFirstValue("email");
        }
    }
}