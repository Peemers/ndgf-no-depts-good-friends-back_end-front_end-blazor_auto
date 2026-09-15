using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ndgf.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExpenseLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Expenses",
                type: "decimal(9,6)",
                precision: 9,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Expenses",
                type: "decimal(9,6)",
                precision: 9,
                scale: 6,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Expenses");
        }
    }
}
