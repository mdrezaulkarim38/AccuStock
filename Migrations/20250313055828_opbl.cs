using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccuStock.Migrations
{
    /// <inheritdoc />
    public partial class Opbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OpeningBalances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OpnDebit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OpnCredit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Debit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ClsDebit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ClsCredit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    ChartOfAccountId = table.Column<int>(type: "int", nullable: false),
                    BusinessYearId = table.Column<int>(type: "int", nullable: false),
                    SubScriptionId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpeningBalances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpeningBalances_BusinessYears_BusinessYearId",
                        column: x => x.BusinessYearId,
                        principalTable: "BusinessYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_OpeningBalances_ChartOfAccounts_ChartOfAccountId",
                        column: x => x.ChartOfAccountId,
                        principalTable: "ChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_OpeningBalances_Subscriptions_SubScriptionId",
                        column: x => x.SubScriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_OpeningBalances_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OpeningBalances_BusinessYearId",
                table: "OpeningBalances",
                column: "BusinessYearId");

            migrationBuilder.CreateIndex(
                name: "IX_OpeningBalances_ChartOfAccountId",
                table: "OpeningBalances",
                column: "ChartOfAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_OpeningBalances_SubScriptionId",
                table: "OpeningBalances",
                column: "SubScriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_OpeningBalances_UserId",
                table: "OpeningBalances",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpeningBalances");
        }
    }
}
