using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccuStock.Migrations
{
    /// <inheritdoc />
    public partial class BusinsessYearSubId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubscriptionId",
                table: "BusinessYears",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessYears_SubscriptionId",
                table: "BusinessYears",
                column: "SubscriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessYears_Subscriptions_SubscriptionId",
                table: "BusinessYears",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessYears_Subscriptions_SubscriptionId",
                table: "BusinessYears");

            migrationBuilder.DropIndex(
                name: "IX_BusinessYears_SubscriptionId",
                table: "BusinessYears");

            migrationBuilder.DropColumn(
                name: "SubscriptionId",
                table: "BusinessYears");
        }
    }
}
