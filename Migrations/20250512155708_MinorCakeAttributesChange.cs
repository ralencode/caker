using System;
using System.Numerics;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Caker.Migrations
{
    /// <inheritdoc />
    public partial class MinorCakeAttributesChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ReqTime",
                table: "Cakes",
                type: "integer",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "interval",
                oldNullable: true);

            migrationBuilder.AlterColumn<BigInteger>(
                name: "Color",
                table: "Cakes",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeSpan>(
                name: "ReqTime",
                table: "Cakes",
                type: "interval",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Color",
                table: "Cakes",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(BigInteger),
                oldType: "numeric",
                oldNullable: true);
        }
    }
}
