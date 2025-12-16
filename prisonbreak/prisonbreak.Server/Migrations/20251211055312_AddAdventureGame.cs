using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace prisonbreak.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddAdventureGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdventureGames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerSessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentRoom = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1),
                    CollectedItemsJson = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "[]"),
                    SolvedPuzzlesJson = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "[]"),
                    Score = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    ElapsedSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    PuzzlesSolved = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdventureGames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdventureGames_Sessions_PlayerSessionId",
                        column: x => x.PlayerSessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdventureGames_PlayerSessionId",
                table: "AdventureGames",
                column: "PlayerSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AdventureGames_Status",
                table: "AdventureGames",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdventureGames");
        }
    }
}
