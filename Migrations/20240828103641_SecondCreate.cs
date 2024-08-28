using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SlotGameBackend.Migrations
{
    /// <inheritdoc />
    public partial class SecondCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "gameSessions",
                columns: table => new
                {
                    sessionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    userId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    sessionStartTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    sessionEndTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    isActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    clientSeed = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    serverSeed = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    lastActivityTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gameSessions", x => x.sessionId);
                    table.ForeignKey(
                        name: "FK_gameSessions_users_userId",
                        column: x => x.userId,
                        principalTable: "users",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "payLines",
                columns: table => new
                {
                    payLineId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    multiplier = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payLines", x => x.payLineId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "symbols",
                columns: table => new
                {
                    symbolId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    symbolName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    imagePath = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_symbols", x => x.symbolId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "wallets",
                columns: table => new
                {
                    walletId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    userId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    balance = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wallets", x => x.walletId);
                    table.ForeignKey(
                        name: "FK_wallets_users_userId",
                        column: x => x.userId,
                        principalTable: "users",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "spinResults",
                columns: table => new
                {
                    spinResultId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    sessionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    betAmount = table.Column<int>(type: "int", nullable: false),
                    winAmount = table.Column<int>(type: "int", nullable: false),
                    serverSeed = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    reelsOutcome = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    spinTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    GameSessionsessionId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_spinResults", x => x.spinResultId);
                    table.ForeignKey(
                        name: "FK_spinResults_gameSessions_GameSessionsessionId",
                        column: x => x.GameSessionsessionId,
                        principalTable: "gameSessions",
                        principalColumn: "sessionId");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "payLinesPositions",
                columns: table => new
                {
                    positionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    X = table.Column<int>(type: "int", nullable: false),
                    Y = table.Column<int>(type: "int", nullable: false),
                    payLineId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payLinesPositions", x => x.positionId);
                    table.ForeignKey(
                        name: "FK_payLinesPositions_payLines_payLineId",
                        column: x => x.payLineId,
                        principalTable: "payLines",
                        principalColumn: "payLineId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    transactionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    walletId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    amount = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    requestedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    transactionStatus = table.Column<int>(type: "int", nullable: false),
                    adminResponse = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ApprovedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserModeluserId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transactions", x => x.transactionId);
                    table.ForeignKey(
                        name: "FK_transactions_users_UserModeluserId",
                        column: x => x.UserModeluserId,
                        principalTable: "users",
                        principalColumn: "userId");
                    table.ForeignKey(
                        name: "FK_transactions_wallets_walletId",
                        column: x => x.walletId,
                        principalTable: "wallets",
                        principalColumn: "walletId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_gameSessions_userId",
                table: "gameSessions",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_payLinesPositions_payLineId",
                table: "payLinesPositions",
                column: "payLineId");

            migrationBuilder.CreateIndex(
                name: "IX_spinResults_GameSessionsessionId",
                table: "spinResults",
                column: "GameSessionsessionId");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_UserModeluserId",
                table: "transactions",
                column: "UserModeluserId");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_walletId",
                table: "transactions",
                column: "walletId");

            migrationBuilder.CreateIndex(
                name: "IX_wallets_userId",
                table: "wallets",
                column: "userId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "payLinesPositions");

            migrationBuilder.DropTable(
                name: "spinResults");

            migrationBuilder.DropTable(
                name: "symbols");

            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DropTable(
                name: "payLines");

            migrationBuilder.DropTable(
                name: "gameSessions");

            migrationBuilder.DropTable(
                name: "wallets");
        }
    }
}
