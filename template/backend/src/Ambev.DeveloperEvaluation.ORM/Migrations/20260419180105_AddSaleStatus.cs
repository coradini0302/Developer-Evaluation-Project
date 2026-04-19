using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ambev.DeveloperEvaluation.ORM.Migrations
{
    /// <inheritdoc />
    public partial class AddSaleStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "Sales");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Sales",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Sales");

            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "Sales",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
