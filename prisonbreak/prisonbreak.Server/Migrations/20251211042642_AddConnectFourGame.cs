using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace prisonbreak.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddConnectFourGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "GameMode",
                table: "TicTacToeGames",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.CreateTable(
                name: "ConnectFourGames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Player1SessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Player2SessionId = table.Column<int>(type: "INTEGER", nullable: true),
                    GameMode = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1),
                    BoardJson = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "[[0,0,0,0,0,0],[0,0,0,0,0,0],[0,0,0,0,0,0],[0,0,0,0,0,0],[0,0,0,0,0,0],[0,0,0,0,0,0],[0,0,0,0,0,0]]"),
                    CurrentPlayer = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    WinnerPlayerId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ElapsedSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    MoveCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConnectFourGames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConnectFourGames_Sessions_Player1SessionId",
                        column: x => x.Player1SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConnectFourGames_Sessions_Player2SessionId",
                        column: x => x.Player2SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConnectFourGames_Player1SessionId",
                table: "ConnectFourGames",
                column: "Player1SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ConnectFourGames_Player2SessionId",
                table: "ConnectFourGames",
                column: "Player2SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ConnectFourGames_Status",
                table: "ConnectFourGames",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConnectFourGames");

            migrationBuilder.AlterColumn<int>(
                name: "GameMode",
                table: "TicTacToeGames",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldDefaultValue: 1);
        }
    }
}
