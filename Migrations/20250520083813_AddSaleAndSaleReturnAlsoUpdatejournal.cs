using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccuStock.Migrations
{
    /// <inheritdoc />
    public partial class AddSaleAndSaleReturnAlsoUpdatejournal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleReturns_SaleDetails_SaleDetailId",
                table: "SaleReturns");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "SaleReturns");

            migrationBuilder.DropColumn(
                name: "RefundAmount",
                table: "SaleReturns");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "SaleReturns");

            migrationBuilder.RenameColumn(
                name: "SaleDetailId",
                table: "SaleReturns",
                newName: "SubscriptionId");

            migrationBuilder.RenameColumn(
                name: "ReturnQuantity",
                table: "SaleReturns",
                newName: "SaleId");

            migrationBuilder.RenameIndex(
                name: "IX_SaleReturns_SaleDetailId",
                table: "SaleReturns",
                newName: "IX_SaleReturns_SubscriptionId");

            migrationBuilder.AddColumn<int>(
                name: "ReturnStatus",
                table: "Sales",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "SaleReturns",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "SaleReturns",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "SaleReturns",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "SaleReturns",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReturnNo",
                table: "SaleReturns",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReturnStatus",
                table: "SaleReturns",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotal",
                table: "SaleReturns",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "SaleReturns",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalVat",
                table: "SaleReturns",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "SaleReturns",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SaleReturnId",
                table: "JournalPosts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SaleReturnId",
                table: "JournalPostDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SaleReturnDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SaleReturnId = table.Column<int>(type: "int", nullable: false),
                    SaleDetailId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VatRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VatAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubscriptionId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleReturnDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleReturnDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_SaleReturnDetails_SaleDetails_SaleDetailId",
                        column: x => x.SaleDetailId,
                        principalTable: "SaleDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_SaleReturnDetails_SaleReturns_SaleReturnId",
                        column: x => x.SaleReturnId,
                        principalTable: "SaleReturns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_SaleReturnDetails_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleReturns_BranchId",
                table: "SaleReturns",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleReturns_CustomerId",
                table: "SaleReturns",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleReturns_SaleId",
                table: "SaleReturns",
                column: "SaleId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalPosts_SaleReturnId",
                table: "JournalPosts",
                column: "SaleReturnId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalPostDetails_SaleReturnId",
                table: "JournalPostDetails",
                column: "SaleReturnId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleReturnDetails_ProductId",
                table: "SaleReturnDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleReturnDetails_SaleDetailId",
                table: "SaleReturnDetails",
                column: "SaleDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleReturnDetails_SaleReturnId",
                table: "SaleReturnDetails",
                column: "SaleReturnId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleReturnDetails_SubscriptionId",
                table: "SaleReturnDetails",
                column: "SubscriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_JournalPostDetails_SaleReturns_SaleReturnId",
                table: "JournalPostDetails",
                column: "SaleReturnId",
                principalTable: "SaleReturns",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JournalPosts_SaleReturns_SaleReturnId",
                table: "JournalPosts",
                column: "SaleReturnId",
                principalTable: "SaleReturns",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleReturns_Branches_BranchId",
                table: "SaleReturns",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleReturns_Customers_CustomerId",
                table: "SaleReturns",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleReturns_Sales_SaleId",
                table: "SaleReturns",
                column: "SaleId",
                principalTable: "Sales",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleReturns_Subscriptions_SubscriptionId",
                table: "SaleReturns",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JournalPostDetails_SaleReturns_SaleReturnId",
                table: "JournalPostDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalPosts_SaleReturns_SaleReturnId",
                table: "JournalPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleReturns_Branches_BranchId",
                table: "SaleReturns");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleReturns_Customers_CustomerId",
                table: "SaleReturns");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleReturns_Sales_SaleId",
                table: "SaleReturns");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleReturns_Subscriptions_SubscriptionId",
                table: "SaleReturns");

            migrationBuilder.DropTable(
                name: "SaleReturnDetails");

            migrationBuilder.DropIndex(
                name: "IX_SaleReturns_BranchId",
                table: "SaleReturns");

            migrationBuilder.DropIndex(
                name: "IX_SaleReturns_CustomerId",
                table: "SaleReturns");

            migrationBuilder.DropIndex(
                name: "IX_SaleReturns_SaleId",
                table: "SaleReturns");

            migrationBuilder.DropIndex(
                name: "IX_JournalPosts_SaleReturnId",
                table: "JournalPosts");

            migrationBuilder.DropIndex(
                name: "IX_JournalPostDetails_SaleReturnId",
                table: "JournalPostDetails");

            migrationBuilder.DropColumn(
                name: "ReturnStatus",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "SaleReturns");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "SaleReturns");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "SaleReturns");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "SaleReturns");

            migrationBuilder.DropColumn(
                name: "ReturnNo",
                table: "SaleReturns");

            migrationBuilder.DropColumn(
                name: "ReturnStatus",
                table: "SaleReturns");

            migrationBuilder.DropColumn(
                name: "SubTotal",
                table: "SaleReturns");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "SaleReturns");

            migrationBuilder.DropColumn(
                name: "TotalVat",
                table: "SaleReturns");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "SaleReturns");

            migrationBuilder.DropColumn(
                name: "SaleReturnId",
                table: "JournalPosts");

            migrationBuilder.DropColumn(
                name: "SaleReturnId",
                table: "JournalPostDetails");

            migrationBuilder.RenameColumn(
                name: "SubscriptionId",
                table: "SaleReturns",
                newName: "SaleDetailId");

            migrationBuilder.RenameColumn(
                name: "SaleId",
                table: "SaleReturns",
                newName: "ReturnQuantity");

            migrationBuilder.RenameIndex(
                name: "IX_SaleReturns_SubscriptionId",
                table: "SaleReturns",
                newName: "IX_SaleReturns_SaleDetailId");

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "SaleReturns",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RefundAmount",
                table: "SaleReturns",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "SaleReturns",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleReturns_SaleDetails_SaleDetailId",
                table: "SaleReturns",
                column: "SaleDetailId",
                principalTable: "SaleDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
