using System.ComponentModel.DataAnnotations;
using IbnAlZumar.Domain.Enums;

namespace IbnAlZumar.API.DTOs.Maintenance;

public class AssignTechnicianDto
{
    [Required]
    public int TechnicianUserId { get; set; }
}

public class ChangeMaintenanceStatusDto
{
    [Required]
    public MaintenanceStatus NewStatus { get; set; }

    /// <summary>Optional — recorded as a MaintenanceNote alongside the transition.</summary>
    public string? Note { get; set; }
}

public class AddMaintenanceNoteDto
{
    [Required, MinLength(1)]
    public string Note { get; set; } = string.Empty;
}

public class AddMaintenancePartUsageDto
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    public int WarehouseId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "الكمية يجب أن تكون 1 على الأقل")]
    public int Quantity { get; set; } = 1;

    /// <summary>Optional — defaults to Product.CurrentCostPrice server-side when omitted.</summary>
    [Range(0, double.MaxValue)]
    public decimal? UnitCostPrice { get; set; }
}

public class SetLaborCostDto
{
    [Range(0, double.MaxValue, ErrorMessage = "قيمة المصنعية يجب أن تكون صفر أو أكبر")]
    public decimal LaborCost { get; set; }
}
