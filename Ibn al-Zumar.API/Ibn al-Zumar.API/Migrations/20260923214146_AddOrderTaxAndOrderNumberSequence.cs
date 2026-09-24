using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ibn_alZumar.API.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderTaxAndOrderNumberSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "Orders",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                table: "Orders",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            // === ضيف السطر ده لتوليد أرقام الفواتير بشكل آمن ===
            migrationBuilder.Sql(
                "CREATE SEQUENCE dbo.OrderNumberSeq AS BIGINT START WITH 1 INCREMENT BY 1 NO CACHE;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // === ضيف السطر ده لحذف الـ Sequence عند التراجع ===
            migrationBuilder.Sql("DROP SEQUENCE dbo.OrderNumberSeq;");

            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                table: "Orders");
        }
    }
}
