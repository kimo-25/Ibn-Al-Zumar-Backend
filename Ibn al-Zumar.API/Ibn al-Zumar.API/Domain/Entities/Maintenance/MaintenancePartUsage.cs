using IbnAlZumar.Domain.Entities.Maintenance;
using IbnAlZumar.Domain.Common;
using IbnAlZumar.Domain.Entities.Catalog;
using IbnAlZumar.Domain.Entities.Inventory;
using System.ComponentModel.DataAnnotations.Schema;

namespace IbnAlZumar.Domain.Entities.Maintenance;

/// <summary>
/// Phase 2 — one spare part consumed by a repair. MaintenanceWorkflowService.AddPartUsageAsync
/// decrements ProductStock.QuantityOnHand and writes a matching InventoryTransaction
/// (TransactionType = MaintenanceUsed, ReferenceType = "MaintenanceRequest",
/// ReferenceId = MaintenanceRequestId) in the same DB transaction as this row — same invariant
/// as every other stock movement in the system (ARCHITECTURE.md §3.2: never mutate
/// ProductStock without a matching InventoryTransaction row in the same transaction).
/// </summary>
public class MaintenancePartUsage : BaseEntity
{
    public int MaintenanceRequestId { get; set; }
    public MaintenanceRequest MaintenanceRequest { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;

    public int Quantity { get; set; } = 1;

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitCostPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal LineTotal { get; set; }

    /// <summary>Links back to the InventoryTransaction row this usage produced, for traceability.</summary>
    public int? InventoryTransactionId { get; set; }
    public InventoryTransaction? InventoryTransaction { get; set; }
}
