using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IbnAlZumar.Domain.Common;
using IbnAlZumar.Domain.Entities.Catalog;
using IbnAlZumar.Domain.Entities.Sales;
using IbnAlZumar.Domain.Enums;

namespace IbnAlZumar.Domain.Entities.Inventory;

/// <summary>
/// Physical or logical stock location. Phase 1 only ever uses Id = 1 ("Main Warehouse"),
/// seeded via HasData in ApplicationDbContext so it's always safe to reference.
/// Phase 2 (multi-warehouse, transfers) just adds more rows — no schema change needed.
///
/// Sheet 1: now supports a 3-tier hierarchy (MainCentral -> RegionalBranch -> PosShelfLocation)
/// via <see cref="Tier"/> and the self-referencing <see cref="ParentWarehouseId"/>. Warehouse Id = 1
/// remains the seeded, always-valid MainCentral default — its ParentWarehouseId stays null.
/// </summary>
public class Warehouse : BaseEntity
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Address { get; set; }

    public bool IsMainWarehouse { get; set; } = false;
    public bool IsActive { get; set; } = true;

    /// <summary>Where this warehouse sits in the 3-tier hierarchy. Defaults to MainCentral.</summary>
    public WarehouseTier Tier { get; set; } = WarehouseTier.MainCentral;

    /// <summary>
    /// Self-referencing FK. Null for a top-level MainCentral warehouse. A RegionalBranch's parent
    /// must be a MainCentral; a PosShelfLocation's parent must be a RegionalBranch (or a MainCentral
    /// for small single-branch setups). Enforced in service logic — see InventoryService.
    /// </summary>
    public int? ParentWarehouseId { get; set; }
    public Warehouse? ParentWarehouse { get; set; }
    public ICollection<Warehouse> ChildWarehouses { get; set; } = new List<Warehouse>();

    public ICollection<ProductStock> ProductStocks { get; set; } = new List<ProductStock>();
    public ICollection<ProductBatch> ProductBatches { get; set; } = new List<ProductBatch>();
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

    /// <summary>
    /// Sheet 1: which batch this movement was drawn from/into, when the product tracks batches.
    /// Null for products that don't use batch tracking, or for movements not attributable to a
    /// single batch (e.g. pre-batch-tracking history).
    /// </summary>
    public int? ProductBatchId { get; set; }
    public ProductBatch? ProductBatch { get; set; }

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

/// <summary>
/// Sheet 1: a received/registered lot of a product with its own expiry, production date and cost.
/// Consumed FEFO (First-Expired-First-Out) on sale/adjustment via InventoryService.ConsumeFefoAsync.
/// RemainingQuantity is the source of truth for "how much of this specific batch is left" — it must
/// only ever change alongside a matching InventoryTransaction row (same invariant as ProductStock).
/// </summary>
public class ProductBatch : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    [Required, MaxLength(100)]
    public string BatchNumber { get; set; } = string.Empty;

    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;

    /// <summary>
    /// Optional — where this batch came from. For legacy/historical stock registered without a
    /// formal Purchase Order, this points at the seeded Opening Balance Supplier
    /// (see DataSeeder.OpeningBalanceSupplierId).
    /// </summary>
    public int? SupplierId { get; set; }

    public DateTime? ProductionDate { get; set; }
    public DateTime ExpiryDate { get; set; }

    public int InitialQuantity { get; set; }
    public int RemainingQuantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal CostPrice { get; set; }

    public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();

    [NotMapped]
    public bool IsDepleted => RemainingQuantity <= 0;

    [NotMapped]
    public bool IsExpired => ExpiryDate.Date < DateTime.UtcNow.Date;
}