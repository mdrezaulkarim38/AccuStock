using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccuStock.Migrations
{
    /// <inheritdoc />
    public partial class addpreturnidinjornal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PurchaseReturnId",
                table: "JournalPosts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PurchaseReturnId",
                table: "JournalPostDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalPosts_PurchaseReturnId",
                table: "JournalPosts",
                column: "PurchaseReturnId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalPostDetails_PurchaseReturnId",
                table: "JournalPostDetails",
                column: "PurchaseReturnId");

            migrationBuilder.AddForeignKey(
                name: "FK_JournalPostDetails_PurchaseReturns_PurchaseReturnId",
                table: "JournalPostDetails",
                column: "PurchaseReturnId",
                principalTable: "PurchaseReturns",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JournalPosts_PurchaseReturns_PurchaseReturnId",
                table: "JournalPosts",
                column: "PurchaseReturnId",
                principalTable: "PurchaseReturns",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JournalPostDetails_PurchaseReturns_PurchaseReturnId",
                table: "JournalPostDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalPosts_PurchaseReturns_PurchaseReturnId",
                table: "JournalPosts");

            migrationBuilder.DropIndex(
                name: "IX_JournalPosts_PurchaseReturnId",
                table: "JournalPosts");

            migrationBuilder.DropIndex(
                name: "IX_JournalPostDetails_PurchaseReturnId",
                table: "JournalPostDetails");

            migrationBuilder.DropColumn(
                name: "PurchaseReturnId",
                table: "JournalPosts");

            migrationBuilder.DropColumn(
                name: "PurchaseReturnId",
                table: "JournalPostDetails");
        }
    }
}
