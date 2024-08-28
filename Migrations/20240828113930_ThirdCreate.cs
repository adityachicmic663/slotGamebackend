using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SlotGameBackend.Migrations
{
    /// <inheritdoc />
    public partial class ThirdCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "settings",
                columns: table => new
                {
                    settingId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    minimumBetLimit = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_settings", x => x.settingId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "settings");
        }
    }
}
