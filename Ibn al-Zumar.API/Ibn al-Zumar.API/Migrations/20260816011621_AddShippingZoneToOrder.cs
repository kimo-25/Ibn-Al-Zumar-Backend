using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ibn_alZumar.API.Migrations
{
    /// <inheritdoc />
    public partial class AddShippingZoneToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShippingZoneId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShippingZoneId",
                table: "Orders",
                column: "ShippingZoneId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_shipping_zones_ShippingZoneId",
                table: "Orders",
                column: "ShippingZoneId",
                principalTable: "shipping_zones",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_shipping_zones_ShippingZoneId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ShippingZoneId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingZoneId",
                table: "Orders");
        }
    }
}
