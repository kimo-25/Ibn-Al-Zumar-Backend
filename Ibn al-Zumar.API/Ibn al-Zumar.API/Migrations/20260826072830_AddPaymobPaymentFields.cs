using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ibn_alZumar.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymobPaymentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymobTransactionId",
                table: "Payments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Payments",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Orders",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymobOrderId",
                table: "Orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymobTransactionId",
                table: "Orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymobTransactionId",
                table: "Payments",
                column: "PaymobTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymobOrderId",
                table: "Orders",
                column: "PaymobOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymobTransactionId",
                table: "Orders",
                column: "PaymobTransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_PaymobTransactionId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PaymobOrderId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PaymobTransactionId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymobTransactionId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymobOrderId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymobTransactionId",
                table: "Orders");
        }
    }
}
