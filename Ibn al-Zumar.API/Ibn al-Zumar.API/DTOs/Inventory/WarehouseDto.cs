using IbnAlZumar.Domain.Enums;

namespace IbnAlZumar.API.DTOs.Inventory
{
    public class WarehouseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsMainWarehouse { get; set; }
        public bool IsActive { get; set; }
        public WarehouseTier Tier { get; set; }
        public string TierName { get; set; } = string.Empty; // Tier.ToString(), for display without a frontend enum map
        public int? ParentWarehouseId { get; set; }
        public string? ParentWarehouseName { get; set; }
    }
}
