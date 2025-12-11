using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace prisonbreak.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddGameModeToTicTacToe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameMode",
                table: "TicTacToeGames",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameMode",
                table: "TicTacToeGames");
        }
    }
}
