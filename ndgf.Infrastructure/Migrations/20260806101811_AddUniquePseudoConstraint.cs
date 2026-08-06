using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ndgf.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniquePseudoConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_Pseudo",
                table: "Users",
                column: "Pseudo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Pseudo",
                table: "Users");
        }
    }
}
