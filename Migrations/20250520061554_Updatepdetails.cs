using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccuStock.Migrations
{
    /// <inheritdoc />
    public partial class Updatepdetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "PurchaseReturnDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reason",
                table: "PurchaseReturnDetails");
        }
    }
}
