using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace prisonbreak.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddWagerToMultiplayerGames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Coins",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 500,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldDefaultValue: 100);

            migrationBuilder.AddColumn<int>(
                name: "Player1Wager",
                table: "TicTacToeGames",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Player2Wager",
                table: "TicTacToeGames",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Player1Wager",
                table: "RockPaperScissorsGames",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Player2Wager",
                table: "RockPaperScissorsGames",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Player1Wager",
                table: "ConnectFourGames",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Player2Wager",
                table: "ConnectFourGames",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Player1Wager",
                table: "TicTacToeGames");

            migrationBuilder.DropColumn(
                name: "Player2Wager",
                table: "TicTacToeGames");

            migrationBuilder.DropColumn(
                name: "Player1Wager",
                table: "RockPaperScissorsGames");

            migrationBuilder.DropColumn(
                name: "Player2Wager",
                table: "RockPaperScissorsGames");

            migrationBuilder.DropColumn(
                name: "Player1Wager",
                table: "ConnectFourGames");

            migrationBuilder.DropColumn(
                name: "Player2Wager",
                table: "ConnectFourGames");

            migrationBuilder.AlterColumn<int>(
                name: "Coins",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 100,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldDefaultValue: 500);
        }
    }
}
