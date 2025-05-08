using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccuStock.Migrations
{
    /// <inheritdoc />
    public partial class Addcolpurchase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ChartOfAccountTypes",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.AddColumn<bool>(
                name: "PaymentMethod",
                table: "Purchases",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Purchases");

            migrationBuilder.InsertData(
                table: "ChartOfAccountTypes",
                columns: new[] { "Id", "GroupID", "Name", "ParentId" },
                values: new object[] { 26, 0, "Accounts Payable - Vendors", 14 });
        }
    }
}
