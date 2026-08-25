using IbnAlZumar.API.DTOs.Sales;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.Domain.Entities.Sales;
using IbnAlZumar.Domain.Enums;
using IbnAlZumar.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IbnAlZumar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingZonesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ShippingZonesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var zones = await _context.ShippingZones.ToListAsync();
            return Ok(zones);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var zone = await _context.ShippingZones.FindAsync(id);
            if (zone == null)
                return NotFound("منطقة الشحن غير موجودة.");

            return Ok(zone);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] ShippingZone zone)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            zone.IsActive = true;
            zone.CreatedAt = DateTime.UtcNow;

            _context.ShippingZones.Add(zone);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = zone.Id }, zone);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] ShippingZone input)
        {
            var zone = await _context.ShippingZones.FindAsync(id);
            if (zone == null)
                return NotFound("منطقة الشحن غير موجودة.");

            zone.Name = input.Name;
            zone.Governorate = input.Governorate;
            zone.ShippingCost = input.ShippingCost;
            zone.ShippingFee = input.ShippingFee;
            zone.EstimatedDays = input.EstimatedDays;
            zone.IsActive = input.IsActive;

            zone.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(zone);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var zone = await _context.ShippingZones.FindAsync(id);
            if (zone == null)
                return NotFound("منطقة الشحن غير موجودة.");

            _context.ShippingZones.Remove(zone);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ================= طلبات مناطق الشحن الجديدة =================
        [HttpGet("pending-requests")]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> GetPendingZoneRequests()
        {
            var requests = await _context.Orders
                .Include(o => o.Customer)
                .Where(o => o.IsCustomZoneRequested
                            && o.CustomZoneRequestStatus == CustomZoneRequestStatus.Pending)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new PendingZoneRequestDto
                {
                    OrderId = o.Id,
                    OrderNumber = o.OrderNumber,
                    CustomZoneName = o.CustomZoneName ?? string.Empty,
                    CustomerName = o.Customer != null ? o.Customer.FullName : (o.GuestName ?? "عميل غير معروف"),
                    CustomerPhone = !string.IsNullOrWhiteSpace(o.GuestPhone)
                        ? o.GuestPhone!
                        : (o.Customer != null ? o.Customer.Phone : null) ?? string.Empty,
                    ShippingAddress = o.ShippingAddress,
                    DeliveryGovernorate = o.DeliveryGovernorate,
                    RequestedAt = o.OrderDate
                })
                .ToListAsync();

            return Ok(requests);
        }

        [HttpPost("requests/{orderId}/accept")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AcceptZoneRequest(int orderId, [FromBody] AcceptZoneRequestDto dto)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
                return NotFound(new { message = "الطلب غير موجود." });

            if (!order.IsCustomZoneRequested || order.CustomZoneRequestStatus != CustomZoneRequestStatus.Pending)
                return BadRequest(new { message = "لا يوجد طلب منطقة شحن قيد الانتظار لهذا الطلب." });

            if (string.IsNullOrWhiteSpace(dto.Governorate))
                return BadRequest(new { message = "يرجى تحديد المحافظة." });

            var zone = new ShippingZone
            {
                Name = !string.IsNullOrWhiteSpace(dto.Name) ? dto.Name.Trim() : (order.CustomZoneName ?? "منطقة جديدة"),
                Governorate = dto.Governorate.Trim(),
                ShippingCost = dto.ShippingCost,
                ShippingFee = dto.ShippingFee,
                EstimatedDays = dto.EstimatedDays > 0 ? dto.EstimatedDays : 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.ShippingZones.Add(zone);
            await _context.SaveChangesAsync();

            order.ShippingZoneId = zone.Id;
            order.DeliveryGovernorate ??= zone.Governorate;
            order.CustomZoneRequestStatus = CustomZoneRequestStatus.Approved;
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم إنشاء منطقة الشحن وربطها بالطلب بنجاح.", zone });
        }

        [HttpPost("requests/{orderId}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectZoneRequest(int orderId, [FromBody] RejectZoneRequestDto? dto)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
                return NotFound(new { message = "الطلب غير موجود." });

            if (!order.IsCustomZoneRequested || order.CustomZoneRequestStatus != CustomZoneRequestStatus.Pending)
                return BadRequest(new { message = "لا يوجد طلب منطقة شحن قيد الانتظار لهذا الطلب." });

            order.CustomZoneRequestStatus = CustomZoneRequestStatus.Rejected;

            if (!string.IsNullOrWhiteSpace(dto?.Reason))
            {
                order.Notes = string.IsNullOrWhiteSpace(order.Notes)
                    ? $"[تم رفض طلب المنطقة]: {dto.Reason}"
                    : $"{order.Notes}\n[تم رفض طلب المنطقة]: {dto.Reason}";
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "تم رفض طلب منطقة الشحن." });
        }
    }
}