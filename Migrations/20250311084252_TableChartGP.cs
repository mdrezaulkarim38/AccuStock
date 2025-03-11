using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccuStock.Migrations
{
    /// <inheritdoc />
    public partial class TableChartGP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupID",
                table: "ChartOfAccountTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "ChartOfAccountTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupID",
                table: "ChartOfAccountTypes");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "ChartOfAccountTypes");
        }
    }
}
