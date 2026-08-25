using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ibn_alZumar.API.Migrations
{
    /// <inheritdoc />
    public partial class FixUserForeignKeyType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminNotes",
                table: "MaintenanceRequests",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaintenanceReportUrl",
                table: "MaintenanceRequests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "MaintenanceRequests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequests_UserId",
                table: "MaintenanceRequests",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceRequests_Users_UserId",
                table: "MaintenanceRequests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceRequests_Users_UserId",
                table: "MaintenanceRequests");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceRequests_UserId",
                table: "MaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "AdminNotes",
                table: "MaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "MaintenanceReportUrl",
                table: "MaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "MaintenanceRequests");
        }
    }
}
