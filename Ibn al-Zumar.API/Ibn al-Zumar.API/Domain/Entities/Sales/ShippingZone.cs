namespace IbnAlZumar.Domain.Entities.Sales
{
    public class ShippingZone
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // اسم المركز أو القرية أو المنطقة
        public string Governorate { get; set; } = string.Empty; // المحافظة التابعة لها
        public decimal ShippingCost { get; set; } // تكلفة الشحن الأساسية
        public decimal ShippingFee { get; set; } // رسوم الشحن الإضافية / الفعلية
        public int EstimatedDays { get; set; } = 1; // عدد الأيام المتوقعة للتوصيل
        public bool IsActive { get; set; } = true; // حالة التفعيل
    }
}