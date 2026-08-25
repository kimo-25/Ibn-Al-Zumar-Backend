using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ibn_alZumar.API.Migrations
{
    /// <inheritdoc />
    public partial class FixPendingModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_ClientUuid",
                table: "Orders");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ClientUuid",
                table: "Orders",
                column: "ClientUuid",
                unique: true,
                filter: "[ClientUuid] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_ClientUuid",
                table: "Orders");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ClientUuid",
                table: "Orders",
                column: "ClientUuid",
                unique: true,
                filter: "\"ClientUuid\" IS NOT NULL");
        }
    }
}
