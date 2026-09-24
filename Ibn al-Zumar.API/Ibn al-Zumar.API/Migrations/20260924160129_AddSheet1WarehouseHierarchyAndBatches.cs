using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ibn_alZumar.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSheet1WarehouseHierarchyAndBatches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Warehouses_Name",
                table: "Warehouses");

            migrationBuilder.AddColumn<int>(
                name: "ParentWarehouseId",
                table: "Warehouses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Tier",
                table: "Warehouses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "ProductVariants",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductBatchId",
                table: "InventoryTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductBatch",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    BatchNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: true),
                    ProductionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InitialQuantity = table.Column<int>(type: "int", nullable: false),
                    RemainingQuantity = table.Column<int>(type: "int", nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductBatch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductBatch_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductBatch_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductVariantAttributeValue",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductVariantId = table.Column<int>(type: "int", nullable: false),
                    ProductAttributeDefinitionId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariantAttributeValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVariantAttributeValue_ProductAttributeDefinitions_ProductAttributeDefinitionId",
                        column: x => x.ProductAttributeDefinitionId,
                        principalTable: "ProductAttributeDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductVariantAttributeValue_ProductVariants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnitConversion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    FromUnit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ToUnit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Factor = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 2, nullable: false),
                    IsBaseUnit = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitConversion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitConversion_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ParentWarehouseId", "Tier" },
                values: new object[] { null, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_ParentWarehouseId",
                table: "Warehouses",
                column: "ParentWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_Tier",
                table: "Warehouses",
                column: "Tier");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_ProductBatchId",
                table: "InventoryTransactions",
                column: "ProductBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBatch_BatchNumber",
                table: "ProductBatch",
                column: "BatchNumber");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBatch_ExpiryDate",
                table: "ProductBatch",
                column: "ExpiryDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBatch_ProductId_WarehouseId_ExpiryDate",
                table: "ProductBatch",
                columns: new[] { "ProductId", "WarehouseId", "ExpiryDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductBatch_WarehouseId",
                table: "ProductBatch",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariantAttributeValue_ProductAttributeDefinitionId",
                table: "ProductVariantAttributeValue",
                column: "ProductAttributeDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariantAttributeValue_ProductVariantId_ProductAttributeDefinitionId",
                table: "ProductVariantAttributeValue",
                columns: new[] { "ProductVariantId", "ProductAttributeDefinitionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnitConversion_ProductId_FromUnit",
                table: "UnitConversion",
                columns: new[] { "ProductId", "FromUnit" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_ProductBatch_ProductBatchId",
                table: "InventoryTransactions",
                column: "ProductBatchId",
                principalTable: "ProductBatch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Warehouses_Warehouses_ParentWarehouseId",
                table: "Warehouses",
                column: "ParentWarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_ProductBatch_ProductBatchId",
                table: "InventoryTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Warehouses_Warehouses_ParentWarehouseId",
                table: "Warehouses");

            migrationBuilder.DropTable(
                name: "ProductBatch");

            migrationBuilder.DropTable(
                name: "ProductVariantAttributeValue");

            migrationBuilder.DropTable(
                name: "UnitConversion");

            migrationBuilder.DropIndex(
                name: "IX_Warehouses_ParentWarehouseId",
                table: "Warehouses");

            migrationBuilder.DropIndex(
                name: "IX_Warehouses_Tier",
                table: "Warehouses");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_ProductBatchId",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "ParentWarehouseId",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "Tier",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "ProductBatchId",
                table: "InventoryTransactions");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_Name",
                table: "Warehouses",
                column: "Name",
                unique: true);
        }
    }
}
