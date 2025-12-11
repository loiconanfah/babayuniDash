using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace prisonbreak.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddRockPaperScissorsGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BattleshipGames");

            migrationBuilder.CreateTable(
                name: "RockPaperScissorsGames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Player1SessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Player2SessionId = table.Column<int>(type: "INTEGER", nullable: true),
                    GameMode = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1),
                    Player1Choice = table.Column<int>(type: "INTEGER", nullable: true),
                    Player2Choice = table.Column<int>(type: "INTEGER", nullable: true),
                    RoundNumber = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1),
                    Player1Score = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    Player2Score = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    RoundsToWin = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 3),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    RoundWinner = table.Column<int>(type: "INTEGER", nullable: true),
                    WinnerPlayerId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ElapsedSeconds = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RockPaperScissorsGames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RockPaperScissorsGames_Sessions_Player1SessionId",
                        column: x => x.Player1SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RockPaperScissorsGames_Sessions_Player2SessionId",
                        column: x => x.Player2SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RockPaperScissorsGames_Player1SessionId",
                table: "RockPaperScissorsGames",
                column: "Player1SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_RockPaperScissorsGames_Player2SessionId",
                table: "RockPaperScissorsGames",
                column: "Player2SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_RockPaperScissorsGames_Status",
                table: "RockPaperScissorsGames",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RockPaperScissorsGames");

            migrationBuilder.CreateTable(
                name: "BattleshipGames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Player1SessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Player2SessionId = table.Column<int>(type: "INTEGER", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CurrentPlayer = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1),
                    ElapsedSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    GameMode = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1),
                    MoveCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Player1BoardJson = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "[]"),
                    Player1Ready = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    Player1ShipsJson = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "[]"),
                    Player1ShotsJson = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "[]"),
                    Player2BoardJson = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "[]"),
                    Player2Ready = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    Player2ShipsJson = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "[]"),
                    Player2ShotsJson = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "[]"),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    WinnerPlayerId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BattleshipGames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BattleshipGames_Sessions_Player1SessionId",
                        column: x => x.Player1SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BattleshipGames_Sessions_Player2SessionId",
                        column: x => x.Player2SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BattleshipGames_Player1SessionId",
                table: "BattleshipGames",
                column: "Player1SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_BattleshipGames_Player2SessionId",
                table: "BattleshipGames",
                column: "Player2SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_BattleshipGames_Status",
                table: "BattleshipGames",
                column: "Status");
        }
    }
}
