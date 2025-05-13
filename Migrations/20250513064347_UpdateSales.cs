using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccuStock.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Tax",
                table: "SaleDetails",
                newName: "VatRate");

            migrationBuilder.RenameColumn(
                name: "Rate",
                table: "SaleDetails",
                newName: "UnitPrice");

            migrationBuilder.RenameColumn(
                name: "Discount",
                table: "SaleDetails",
                newName: "Total");

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotal",
                table: "Sales",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalVat",
                table: "Sales",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "SaleDetails",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotal",
                table: "SaleDetails",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionId",
                table: "SaleDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "SaleDetails",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "VatAmount",
                table: "SaleDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "PurchaseDetails",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "PurchaseDetails",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_SaleDetails_SubscriptionId",
                table: "SaleDetails",
                column: "SubscriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleDetails_Subscriptions_SubscriptionId",
                table: "SaleDetails",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleDetails_Subscriptions_SubscriptionId",
                table: "SaleDetails");

            migrationBuilder.DropIndex(
                name: "IX_SaleDetails_SubscriptionId",
                table: "SaleDetails");

            migrationBuilder.DropColumn(
                name: "SubTotal",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "TotalVat",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "SaleDetails");

            migrationBuilder.DropColumn(
                name: "SubTotal",
                table: "SaleDetails");

            migrationBuilder.DropColumn(
                name: "SubscriptionId",
                table: "SaleDetails");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "SaleDetails");

            migrationBuilder.DropColumn(
                name: "VatAmount",
                table: "SaleDetails");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "PurchaseDetails");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "PurchaseDetails");

            migrationBuilder.RenameColumn(
                name: "VatRate",
                table: "SaleDetails",
                newName: "Tax");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "SaleDetails",
                newName: "Rate");

            migrationBuilder.RenameColumn(
                name: "Total",
                table: "SaleDetails",
                newName: "Discount");
        }
    }
}
