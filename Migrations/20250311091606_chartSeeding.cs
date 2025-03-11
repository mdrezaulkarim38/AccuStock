using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AccuStock.Migrations
{
    /// <inheritdoc />
    public partial class chartSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ChartOfAccountTypes",
                columns: new[] { "Id", "GroupID", "Name", "ParentId" },
                values: new object[,]
                {
                    { 1, 1, "Assets", 0 },
                    { 2, 1, "Liabilities", 0 },
                    { 3, 1, "Equity", 0 },
                    { 4, 1, "Income", 0 },
                    { 5, 1, "Expense", 0 },
                    { 6, 0, "Other Assets", 1 },
                    { 7, 0, "Other Current Assets", 1 },
                    { 8, 0, "Cash", 1 },
                    { 9, 0, "Bank", 1 },
                    { 10, 0, "Fixed Assets", 1 },
                    { 11, 0, "Stock", 1 },
                    { 12, 0, "Payment Clearing", 1 },
                    { 13, 0, "Input Tax", 1 },
                    { 14, 0, "Other Current Liability", 2 },
                    { 15, 0, "Credit Card", 2 },
                    { 16, 0, "Long Term Liability", 2 },
                    { 17, 0, "Other Liability", 2 },
                    { 18, 0, "Overseas Tax Payable", 2 },
                    { 19, 0, "Output Tax", 2 },
                    { 20, 0, "Equity", 3 },
                    { 21, 0, "Income", 4 },
                    { 22, 0, "Other Income", 4 },
                    { 23, 0, "Expense", 5 },
                    { 24, 0, "Cost of Goods Sold", 5 },
                    { 25, 0, "Other Expense", 5 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ChartOfAccountTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ChartOfAccountTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ChartOfAccountTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ChartOfAccountTypes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ChartOfAccountTypes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ChartOfAccountTypes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ChartOfAccountTypes",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ChartOfAccountTypes",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ChartOfAccountTypes",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ChartOfAccountTypes",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ChartOfAccountTypes",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "ChartOfAccountTypes",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "ChartOfAccountTypes",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "ChartOfAccountTypes",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "ChartOfAccountTypes",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "ChartOfAccountTypes",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "ChartOfAccountTypes",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "ChartOfAccountTypes",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "ChartOfAccountTypes",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "ChartOfAccountTypes",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "ChartOfAccountTypes",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "ChartOfAccountTypes",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "ChartOfAccountTypes",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "ChartOfAccountTypes",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "ChartOfAccountTypes",
                keyColumn: "Id",
                keyValue: 25);
        }
    }
}
