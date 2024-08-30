using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SlotGameBackend.Migrations
{
    /// <inheritdoc />
    public partial class Addblocking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "serverSeed",
                table: "spinResults");

            migrationBuilder.AddColumn<bool>(
                name: "isBlocked",
                table: "users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isBlocked",
                table: "users");

            migrationBuilder.AddColumn<Guid>(
                name: "serverSeed",
                table: "spinResults",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");
        }
    }
}
