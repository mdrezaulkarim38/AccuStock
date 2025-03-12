using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccuStock.Migrations
{
    /// <inheritdoc />
    public partial class ColRemove : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChartOfAccounts_ChartOfAccountTypes_ChartOfAccountTypeId",
                table: "ChartOfAccounts");

            migrationBuilder.DropColumn(
                name: "AccountTypeId",
                table: "ChartOfAccounts");

            migrationBuilder.AlterColumn<int>(
                name: "ChartOfAccountTypeId",
                table: "ChartOfAccounts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChartOfAccounts_ChartOfAccountTypes_ChartOfAccountTypeId",
                table: "ChartOfAccounts",
                column: "ChartOfAccountTypeId",
                principalTable: "ChartOfAccountTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChartOfAccounts_ChartOfAccountTypes_ChartOfAccountTypeId",
                table: "ChartOfAccounts");

            migrationBuilder.AlterColumn<int>(
                name: "ChartOfAccountTypeId",
                table: "ChartOfAccounts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AccountTypeId",
                table: "ChartOfAccounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_ChartOfAccounts_ChartOfAccountTypes_ChartOfAccountTypeId",
                table: "ChartOfAccounts",
                column: "ChartOfAccountTypeId",
                principalTable: "ChartOfAccountTypes",
                principalColumn: "Id");
        }
    }
}
