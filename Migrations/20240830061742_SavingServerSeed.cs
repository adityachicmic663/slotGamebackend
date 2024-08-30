using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SlotGameBackend.Migrations
{
    /// <inheritdoc />
    public partial class SavingServerSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "clientSeed",
                table: "spinResults",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "clientSeed",
                table: "spinResults");
        }
    }
}
