namespace IbnAlZumar.API.DTOs.Inventory
{
    public class WarehouseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsMainWarehouse { get; set; }
        public bool IsActive { get; set; }
    }
}
