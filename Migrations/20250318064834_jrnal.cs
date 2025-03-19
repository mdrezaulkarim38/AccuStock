using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccuStock.Migrations
{
    /// <inheritdoc />
    public partial class Jrnal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JournalPosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessYearId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    VchNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VchDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VchType = table.Column<int>(type: "int", nullable: true),
                    Debit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: true),
                    PurchaseId = table.Column<int>(type: "int", nullable: true),
                    VendorPaymentId = table.Column<int>(type: "int", nullable: true),
                    SaleId = table.Column<int>(type: "int", nullable: true),
                    CustomerPaymentId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    RefNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SubscriptionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JournalPosts_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_JournalPosts_BusinessYears_BusinessYearId",
                        column: x => x.BusinessYearId,
                        principalTable: "BusinessYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_JournalPosts_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_JournalPosts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JournalPostDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessYearId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    JournalPostId = table.Column<int>(type: "int", nullable: false),
                    ChartOfAccountId = table.Column<int>(type: "int", nullable: false),
                    VchNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VchDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VchType = table.Column<int>(type: "int", nullable: true),
                    Debit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ChqNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChqDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    PurchaseId = table.Column<int>(type: "int", nullable: true),
                    VendorPaymentId = table.Column<int>(type: "int", nullable: true),
                    SaleId = table.Column<int>(type: "int", nullable: true),
                    CustomerPaymentId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubscriptionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalPostDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JournalPostDetails_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_JournalPostDetails_BusinessYears_BusinessYearId",
                        column: x => x.BusinessYearId,
                        principalTable: "BusinessYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_JournalPostDetails_ChartOfAccounts_ChartOfAccountId",
                        column: x => x.ChartOfAccountId,
                        principalTable: "ChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_JournalPostDetails_JournalPosts_JournalPostId",
                        column: x => x.JournalPostId,
                        principalTable: "JournalPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_JournalPostDetails_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JournalPostDetails_BranchId",
                table: "JournalPostDetails",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalPostDetails_BusinessYearId",
                table: "JournalPostDetails",
                column: "BusinessYearId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalPostDetails_ChartOfAccountId",
                table: "JournalPostDetails",
                column: "ChartOfAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalPostDetails_JournalPostId",
                table: "JournalPostDetails",
                column: "JournalPostId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalPostDetails_SubscriptionId",
                table: "JournalPostDetails",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalPosts_BranchId",
                table: "JournalPosts",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalPosts_BusinessYearId",
                table: "JournalPosts",
                column: "BusinessYearId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalPosts_SubscriptionId",
                table: "JournalPosts",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalPosts_UserId",
                table: "JournalPosts",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JournalPostDetails");

            migrationBuilder.DropTable(
                name: "JournalPosts");
        }
    }
}
