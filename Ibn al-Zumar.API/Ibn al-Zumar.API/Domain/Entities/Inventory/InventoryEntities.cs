using System.ComponentModel.DataAnnotations;
using IbnAlZumar.Domain.Common;
using IbnAlZumar.Domain.Entities.Catalog;
using IbnAlZumar.Domain.Entities.Sales;
using IbnAlZumar.Domain.Enums;

namespace IbnAlZumar.Domain.Entities.Inventory;

/// <summary>
/// Physical or logical stock location. Phase 1 only ever uses Id = 1 ("Main Warehouse"),
/// seeded via HasData in ApplicationDbContext so it's always safe to reference.
/// Phase 2 (multi-warehouse, transfers) just adds more rows — no schema change needed.
/// </summary>
public class Warehouse : BaseEntity
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Address { get; set; }

    public bool IsMainWarehouse { get; set; } = false;
    public bool IsActive { get; set; } = true;

    public ICollection<ProductStock> ProductStocks { get; set; } = new List<ProductStock>();
    public ICollection<StockTransfer> OutgoingTransfers { get; set; } = new List<StockTransfer>();
    public ICollection<StockTransfer> IncomingTransfers { get; set; } = new List<StockTransfer>();
}

/// <summary>
/// One row per (Product, Warehouse) pair. In Phase 1 every product has exactly one row
/// pointing at Warehouse Id = 1. Unique composite index enforces one row per pair.
/// </summary>
public class ProductStock : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;

    public int QuantityOnHand { get; set; }
    public int ReorderLevel { get; set; }

    public DateTime? LastRestockedAt { get; set; }
}

/// <summary>
/// Append-only audit ledger for every stock movement (purchase received, sale, transfer,
/// manual adjustment, return...). Nothing should ever update QuantityOnHand directly without
/// writing a matching row here — this is what makes stock levels explainable/auditable later.
/// </summary>
public class InventoryTransaction : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;

    public InventoryTransactionType TransactionType { get; set; }

    /// <summary>Signed quantity delta: positive for stock in, negative for stock out.</summary>
    public int QuantityChange { get; set; }

    /// <summary>Loose polymorphic reference, e.g. "Order", "PurchaseOrder", "StockTransfer".</summary>
    [MaxLength(50)]
    public string? ReferenceType { get; set; }
    public int? ReferenceId { get; set; }

    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? Notes { get; set; }
}

/// <summary>Phase 2: moving stock from one warehouse to another. Header row.</summary>
public class StockTransfer : BaseEntity
{
    public int SourceWarehouseId { get; set; }
    public Warehouse SourceWarehouse { get; set; } = null!;

    public int DestinationWarehouseId { get; set; }
    public Warehouse DestinationWarehouse { get; set; } = null!;

    public StockTransferStatus Status { get; set; } = StockTransferStatus.Requested;

    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public ICollection<StockTransferItem> Items { get; set; } = new List<StockTransferItem>();
}

public class StockTransferItem : BaseEntity
{
    public int StockTransferId { get; set; }
    public StockTransfer StockTransfer { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }
}
