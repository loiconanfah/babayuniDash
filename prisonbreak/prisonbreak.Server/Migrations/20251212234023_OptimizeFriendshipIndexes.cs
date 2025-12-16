using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace prisonbreak.Server.Migrations
{
    /// <inheritdoc />
    public partial class OptimizeFriendshipIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Friendships_Status",
                table: "Friendships",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_UserId_Status",
                table: "Friendships",
                columns: new[] { "UserId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Friendships_Status",
                table: "Friendships");

            migrationBuilder.DropIndex(
                name: "IX_Friendships_UserId_Status",
                table: "Friendships");
        }
    }
}
