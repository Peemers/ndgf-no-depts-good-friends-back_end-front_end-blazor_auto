using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ndgf.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixExpensePartsNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseParts_Expenses_ExpenseId1",
                table: "ExpenseParts");

            migrationBuilder.DropIndex(
                name: "IX_ExpenseParts_ExpenseId1",
                table: "ExpenseParts");

            migrationBuilder.DropColumn(
                name: "ExpenseId1",
                table: "ExpenseParts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ExpenseId1",
                table: "ExpenseParts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseParts_ExpenseId1",
                table: "ExpenseParts",
                column: "ExpenseId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseParts_Expenses_ExpenseId1",
                table: "ExpenseParts",
                column: "ExpenseId1",
                principalTable: "Expenses",
                principalColumn: "Id");
        }
    }
}
