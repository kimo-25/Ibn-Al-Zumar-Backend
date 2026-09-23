using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace IbnAlZumar.API.DTOs.Maintenance;

public sealed class CreateMaintenanceRequestDto
{
    [Required(ErrorMessage = "وصف المشكلة مطلوب")]
    public string Description { get; set; } = string.Empty;
    public int DeliveryMethod { get; set; }
    public IFormFile? Image { get; set; }
    public List<IFormFile> Images { get; set; } = new();
}





