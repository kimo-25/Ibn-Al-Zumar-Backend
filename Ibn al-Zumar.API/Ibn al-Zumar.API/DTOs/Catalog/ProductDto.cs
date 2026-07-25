namespace IbnAlZumar.API.DTOs.Catalog
{
    // الـ DTO اللي هترجعه للـ Frontend
    public record ProductDto(int Id, string Name, decimal Price, int StockQuantity);
}