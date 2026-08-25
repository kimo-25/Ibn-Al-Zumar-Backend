using System.Security.Claims;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.Domain.Entities.Maintenance;
using IbnAlZumar.Domain.Enums;

namespace IbnAlZumar.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MaintenanceController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public MaintenanceController(ApplicationDbContext context, IConfiguration configuration, IWebHostEnvironment environment)
    {
        _context = context;
        _configuration = configuration;
        _environment = environment;
    }

    [HttpPost]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateRequest([FromForm] string description, [FromForm] int deliveryMethod, IFormFile? image)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        int? userId = int.TryParse(userIdStr, out int parsedId) ? parsedId : null;

        int? customerId = null;
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        if (!string.IsNullOrEmpty(userEmail))
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email != null && c.Email.ToLower() == userEmail.ToLower());
            customerId = customer?.Id;
        }

        string? imageUrl = null;

        if (image != null)
        {
            // حفظ الصورة في مجلد uploads الخارجي المعرف في Program.cs بدلاً من wwwroot
            var uploadsFolder = Path.Combine(_environment.ContentRootPath, "uploads", "maintenance");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var extension = Path.GetExtension(image.FileName);
            if (string.IsNullOrWhiteSpace(extension) || extension.Length > 10)
            {
                extension = ".png";
            }
            var uniqueName = $"{Guid.NewGuid():N}{extension}";

            var filePath = Path.Combine(uploadsFolder, uniqueName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            // مسار نسبي آمن بدون سلاش زائدة في البداية
            imageUrl = $"uploads/maintenance/{uniqueName}";
        }

        var request = new MaintenanceRequest
        {
            UserId = userId,
            CustomerId = customerId,
            ProblemDescription = description,
            DeliveryMethod = (DeliveryMethod)deliveryMethod,
            ImageUrl = imageUrl,
            Status = MaintenanceStatus.Pending
        };

        _context.MaintenanceRequests.Add(request);
        await _context.SaveChangesAsync();

        return Ok(new { message = "تم إرسال طلب الصيانة بنجاح وسيتم تحديد السعر والموعد قريباً." });
    }

    [HttpGet("my-requests")]
    [Authorize]
    public async Task<IActionResult> GetMyRequests()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        int.TryParse(userIdStr, out int userId);

        var requests = await _context.MaintenanceRequests
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new
            {
                r.Id,
                r.ProblemDescription,
                r.ImageUrl,
                r.DeliveryMethod,
                r.Status,
                r.EstimatedPrice,
                r.ScheduledDate,
                r.AdminNotes,
                r.MaintenanceReportUrl,
                r.CreatedAt
            })
            .ToListAsync();

        return Ok(requests);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,SuperAdmin,admin,MODERATOR,STORE_OWNER")]
    public async Task<IActionResult> GetAllRequests()
    {
        var requests = await _context.MaintenanceRequests
            .Include(r => r.User)
            .Include(r => r.Customer)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new
            {
                r.Id,
                r.UserId,
                r.CustomerId,
                UserName = r.User != null ? r.User.FullName : (r.Customer != null ? r.Customer.FullName : "عميل غير مسجل"),
                UserPhone = r.Customer != null ? r.Customer.Phone : (r.User != null ? (r.User.PendingPhone ?? "") : ""),
                UserEmail = r.User != null ? r.User.Email : (r.Customer != null ? r.Customer.Email : ""),
                r.ProblemDescription,
                r.ImageUrl,
                r.DeliveryMethod,
                r.Status,
                r.EstimatedPrice,
                r.ScheduledDate,
                r.AdminNotes,
                r.MaintenanceReportUrl,
                r.CreatedAt
            })
            .ToListAsync();

        return Ok(requests);
    }

    [HttpPut("{id}/respond")]
    [Authorize(Roles = "Admin,SuperAdmin,admin,MODERATOR,STORE_OWNER")]
    public async Task<IActionResult> RespondToMaintenance(int id, [FromBody] MaintenanceResponseDto dto)
    {
        var request = await _context.MaintenanceRequests
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request == null) return NotFound("طلب الصيانة غير موجود");

        request.Status = (MaintenanceStatus)dto.Status;
        request.EstimatedPrice = dto.EstimatedPrice;
        request.ScheduledDate = dto.ScheduledDate;
        request.AdminNotes = dto.AdminNotes;
        request.MaintenanceReportUrl = dto.MaintenanceReportUrl;

        await _context.SaveChangesAsync();

        if (request.User != null && !string.IsNullOrEmpty(request.User.Email))
        {
            _ = Task.Run(() => SendStatusEmailNotification(request.User.Email, request.Id, dto.AdminNotes, dto.EstimatedPrice));
        }

        return Ok(new { message = "تم تحديث طلب الصيانة وإشعار العميل بنجاح." });
    }

    private void SendStatusEmailNotification(string toEmail, int requestId, string? notes, decimal? price)
    {
        try
        {
            var smtpServer = _configuration["Smtp:Host"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_configuration["Smtp:Port"] ?? "587");
            var smtpUser = _configuration["Smtp:Username"];
            var smtpPass = _configuration["Smtp:Password"];

            if (string.IsNullOrEmpty(smtpUser)) return;

            using var mail = new MailMessage();
            mail.From = new MailAddress(smtpUser, "ابن الزمر للصيانة");
            mail.To.Add(toEmail);
            mail.Subject = $"تحديث بشأن طلب الصيانة #{requestId}";
            mail.Body = $@"
                <div dir='rtl' style='font-family: Arial; padding: 20px;'>
                    <h2>تحديث جديد بخصوص طلب الصيانة الخاص بك (#{requestId})</h2>
                    <p><strong>رد مهندس الصيانة:</strong> {notes ?? "تمت مراجعة الطلب."}</p>
                    {(price.HasValue ? $"<p><strong>التكلفة التقديرية:</strong> {price.Value} ج.م</p>" : "")}
                    <br/>
                    <p>يمكنك متابعة حالة الطلب والتقرير كاملاً عبر حسابك بالمتجر.</p>
                </div>";
            mail.IsBodyHtml = true;

            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };
            client.Send(mail);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"فشل إرسال الإيميل: {ex.Message}");
        }
    }
}

public class MaintenanceResponseDto
{
    public int Status { get; set; }
    public decimal? EstimatedPrice { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public string? AdminNotes { get; set; }
    public string? MaintenanceReportUrl { get; set; }
}