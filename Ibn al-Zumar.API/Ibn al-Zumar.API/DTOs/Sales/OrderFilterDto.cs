using IbnAlZumar.Domain.Enums;
using System;

namespace IbnAlZumar.API.DTOs.Sales;

public class OrderFilterDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 30;

    public string? SearchTerm { get; set; }

    public OrderStatus? Status { get; set; }

    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    public string? SortBy { get; set; } // e.g. "createdAt", "total", "orderNumber"
    public bool SortDescending { get; set; } = true;
}