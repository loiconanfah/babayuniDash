using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace prisonbreak.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlToTournament : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Tournaments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UserId_IsActive_ExpiresAt",
                table: "Sessions",
                columns: new[] { "UserId", "IsActive", "ExpiresAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sessions_UserId_IsActive_ExpiresAt",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Tournaments");
        }
    }
}
