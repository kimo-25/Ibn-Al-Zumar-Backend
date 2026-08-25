using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ibn_alZumar.API.Migrations
{
    /// <inheritdoc />
    public partial class FixProfileChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_shipping_zones_ShippingZoneId",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_shipping_zones",
                table: "shipping_zones");

            migrationBuilder.RenameTable(
                name: "shipping_zones",
                newName: "ShippingZones");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShippingZones",
                table: "ShippingZones",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_ShippingZones_ShippingZoneId",
                table: "Orders",
                column: "ShippingZoneId",
                principalTable: "ShippingZones",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_ShippingZones_ShippingZoneId",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShippingZones",
                table: "ShippingZones");

            migrationBuilder.RenameTable(
                name: "ShippingZones",
                newName: "shipping_zones");

            migrationBuilder.AddPrimaryKey(
                name: "PK_shipping_zones",
                table: "shipping_zones",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_shipping_zones_ShippingZoneId",
                table: "Orders",
                column: "ShippingZoneId",
                principalTable: "shipping_zones",
                principalColumn: "Id");
        }
    }
}
