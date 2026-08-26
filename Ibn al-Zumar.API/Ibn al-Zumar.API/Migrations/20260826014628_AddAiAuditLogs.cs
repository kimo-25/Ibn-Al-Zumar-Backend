using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ibn_alZumar.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAiAuditLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AiAuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    UserEmail = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: true),
                    Roles = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Prompt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToolName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Succeeded = table.Column<bool>(type: "bit", nullable: false),
                    Error = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    TimestampUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiAuditLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AiAuditLogs_TimestampUtc",
                table: "AiAuditLogs",
                column: "TimestampUtc");

            migrationBuilder.CreateIndex(
                name: "IX_AiAuditLogs_ToolName",
                table: "AiAuditLogs",
                column: "ToolName");

            migrationBuilder.CreateIndex(
                name: "IX_AiAuditLogs_UserId_TimestampUtc",
                table: "AiAuditLogs",
                columns: new[] { "UserId", "TimestampUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AiAuditLogs");
        }
    }
}
