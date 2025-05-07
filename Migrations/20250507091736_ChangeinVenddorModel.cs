using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccuStock.Migrations
{
    /// <inheritdoc />
    public partial class ChangeinVenddorModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChartOfAccountId",
                table: "Vendors",
                type: "int",
                nullable: true);

            migrationBuilder.InsertData(
                table: "ChartOfAccountTypes",
                columns: new[] { "Id", "GroupID", "Name", "ParentId" },
                values: new object[] { 26, 0, "Accounts Payable - Vendors", 14 });

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_ChartOfAccountId",
                table: "Vendors",
                column: "ChartOfAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_ChartOfAccounts_ChartOfAccountId",
                table: "Vendors",
                column: "ChartOfAccountId",
                principalTable: "ChartOfAccounts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_ChartOfAccounts_ChartOfAccountId",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_Vendors_ChartOfAccountId",
                table: "Vendors");

            migrationBuilder.DeleteData(
                table: "ChartOfAccountTypes",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DropColumn(
                name: "ChartOfAccountId",
                table: "Vendors");
        }
    }
}
