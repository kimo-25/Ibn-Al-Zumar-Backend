using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ibn_alZumar.API.Migrations
{
    /// <inheritdoc />
    public partial class Phase2_MaintenanceWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ActualCost",
                table: "MaintenanceRequests",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssignedTechnicianUserId",
                table: "MaintenanceRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveredAt",
                table: "MaintenanceRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LaborCost",
                table: "MaintenanceRequests",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "MaintenanceNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaintenanceRequestId = table.Column<int>(type: "int", nullable: false),
                    AuthorUserId = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusAtNote = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceNotes_MaintenanceRequests_MaintenanceRequestId",
                        column: x => x.MaintenanceRequestId,
                        principalTable: "MaintenanceRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaintenanceNotes_Users_AuthorUserId",
                        column: x => x.AuthorUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaintenancePartUsages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaintenanceRequestId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitCostPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    InventoryTransactionId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenancePartUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenancePartUsages_InventoryTransactions_InventoryTransactionId",
                        column: x => x.InventoryTransactionId,
                        principalTable: "InventoryTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaintenancePartUsages_MaintenanceRequests_MaintenanceRequestId",
                        column: x => x.MaintenanceRequestId,
                        principalTable: "MaintenanceRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaintenancePartUsages_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaintenancePartUsages_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequests_AssignedTechnicianUserId",
                table: "MaintenanceRequests",
                column: "AssignedTechnicianUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceNotes_AuthorUserId",
                table: "MaintenanceNotes",
                column: "AuthorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceNotes_MaintenanceRequestId_CreatedAt",
                table: "MaintenanceNotes",
                columns: new[] { "MaintenanceRequestId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePartUsages_InventoryTransactionId",
                table: "MaintenancePartUsages",
                column: "InventoryTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePartUsages_MaintenanceRequestId",
                table: "MaintenancePartUsages",
                column: "MaintenanceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePartUsages_ProductId",
                table: "MaintenancePartUsages",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePartUsages_WarehouseId",
                table: "MaintenancePartUsages",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceRequests_Users_AssignedTechnicianUserId",
                table: "MaintenanceRequests",
                column: "AssignedTechnicianUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceRequests_Users_AssignedTechnicianUserId",
                table: "MaintenanceRequests");

            migrationBuilder.DropTable(
                name: "MaintenanceNotes");

            migrationBuilder.DropTable(
                name: "MaintenancePartUsages");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceRequests_AssignedTechnicianUserId",
                table: "MaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "ActualCost",
                table: "MaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "AssignedTechnicianUserId",
                table: "MaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "DeliveredAt",
                table: "MaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "LaborCost",
                table: "MaintenanceRequests");
        }
    }
}
