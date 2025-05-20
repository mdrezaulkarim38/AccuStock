using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccuStock.Migrations
{
    /// <inheritdoc />
    public partial class NewReturnPurchase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReturnStatus",
                table: "Purchases",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReturnStatus",
                table: "Purchases");
        }
    }
}
