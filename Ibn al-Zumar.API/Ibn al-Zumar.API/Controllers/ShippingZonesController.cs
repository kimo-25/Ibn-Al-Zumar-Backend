using IbnAlZumar.API.Persistence;
using IbnAlZumar.Domain.Entities.Sales;
using IbnAlZumar.Persistence; // اضبط الـ namespace الخاص بالـ DbContext طبقاً لمشروعك
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

        // جلب جميع مناطق الشحن
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var zones = await _context.ShippingZones.ToListAsync();
            return Ok(zones);
        }

        // جلب منطقة شحن محددة بواسطة الـ ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var zone = await _context.ShippingZones.FindAsync(id);
            if (zone == null)
                return NotFound("منطقة الشحن غير موجودة.");

            return Ok(zone);
        }

        // إضافة منطقة شحن جديدة (للأدمن)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] ShippingZone zone)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.ShippingZones.Add(zone);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = zone.Id }, zone);
        }

        // تعديل بيانات منطقة الشحن
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

            await _context.SaveChangesAsync();
            return Ok(zone);
        }

        // حذف منطقة شحن
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
    }
}