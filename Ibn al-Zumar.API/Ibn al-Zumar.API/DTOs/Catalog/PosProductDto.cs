namespace IbnAlZumar.API.DTOs.Catalog;

/// <summary>
/// One product as the POS screen needs it: base info + every variant, every unit conversion,
/// and every configured pricing-tier price break, plus the on-hand quantity for the requested
/// warehouse. This is a READ model only — never used to trust a client-supplied price; the
/// server still re-resolves the authoritative unit price at order creation (see PricingService).
/// </summary>
public class PosProductDto
{
    public int Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? ImageUrl { get; set; }
    public decimal SellingPrice { get; set; }
    public bool TrackInventory { get; set; }
    public decimal QuantityOnHand { get; set; }
    public List<PosUnitConversionDto> Units { get; set; } = new();
    public List<PosVariantDto> Variants { get; set; } = new();
    public List<ProductPriceDto> PricingTiers { get; set; } = new();
}

public class PosUnitConversionDto
{
    public int Id { get; set; }
    public string FromUnit { get; set; } = string.Empty;
    public string ToUnit { get; set; } = string.Empty;
    public decimal Factor { get; set; }
    public bool IsBaseUnit { get; set; }
}

public class PosVariantDto
{
    public int Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? Color { get; set; }
    public string? Finish { get; set; }
    public string? Material { get; set; }
    public string? Size { get; set; }
    public bool IsActive { get; set; }
}
