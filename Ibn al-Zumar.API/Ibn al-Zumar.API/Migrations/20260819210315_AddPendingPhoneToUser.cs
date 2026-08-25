using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ibn_alZumar.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPendingPhoneToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhoneVerificationExpiry",
                table: "Users",
                newName: "PendingPhoneExpiry");

            migrationBuilder.RenameColumn(
                name: "PhoneVerificationCode",
                table: "Users",
                newName: "PendingPhoneCode");

            migrationBuilder.AddColumn<string>(
                name: "PendingPhone",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PendingPhone",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "PendingPhoneExpiry",
                table: "Users",
                newName: "PhoneVerificationExpiry");

            migrationBuilder.RenameColumn(
                name: "PendingPhoneCode",
                table: "Users",
                newName: "PhoneVerificationCode");
        }
    }
}
