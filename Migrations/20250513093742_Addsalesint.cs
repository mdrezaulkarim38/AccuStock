﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccuStock.Migrations
{
    /// <inheritdoc />
    public partial class Addsalesint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PaymentMethod",
                table: "Sales",
                type: "int",
                maxLength: 60,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "Sales",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 60,
                oldNullable: true);
        }
    }
}
