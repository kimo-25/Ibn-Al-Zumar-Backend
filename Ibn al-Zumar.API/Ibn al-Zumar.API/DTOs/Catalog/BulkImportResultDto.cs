// File: DTOs/Catalog/BulkImportResultDto.cs
// Place this next to CreateProductDto.cs in the same folder/namespace.

namespace IbnAlZumar.API.DTOs.Catalog;

public class BulkImportResultDto
{
    public int TotalRows { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public List<string> ImportedSkus { get; set; } = new();
    public List<BulkImportRowErrorDto> Errors { get; set; } = new();
}

public class BulkImportRowErrorDto
{
    public int RowNumber { get; set; }
    public string? SKU { get; set; }
    public List<string> Errors { get; set; } = new();
}