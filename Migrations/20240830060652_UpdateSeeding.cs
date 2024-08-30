using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SlotGameBackend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "clientSeed",
                table: "gameSessions");

            migrationBuilder.AddColumn<Guid>(
                name: "serverSeed",
                table: "spinResults",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "serverSeed",
                table: "spinResults");

            migrationBuilder.AddColumn<string>(
                name: "clientSeed",
                table: "gameSessions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
