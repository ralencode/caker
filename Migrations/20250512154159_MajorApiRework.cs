using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Caker.Migrations
{
    /// <inheritdoc />
    public partial class MajorApiRework : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Tastes",
                table: "Confectioners",
                newName: "Fillings");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsCustom",
                table: "Orders",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "DoImages",
                table: "Confectioners",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DoShapes",
                table: "Confectioners",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxEta",
                table: "Confectioners",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinEta",
                table: "Confectioners",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Price",
                table: "Cakes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<long>(
                name: "Color",
                table: "Cakes",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "Fillings",
                table: "Cakes",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Cakes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsCustom",
                table: "Cakes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "TextSize",
                table: "Cakes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TextX",
                table: "Cakes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TextY",
                table: "Cakes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsCustom",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DoImages",
                table: "Confectioners");

            migrationBuilder.DropColumn(
                name: "DoShapes",
                table: "Confectioners");

            migrationBuilder.DropColumn(
                name: "MaxEta",
                table: "Confectioners");

            migrationBuilder.DropColumn(
                name: "MinEta",
                table: "Confectioners");

            migrationBuilder.DropColumn(
                name: "Fillings",
                table: "Cakes");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Cakes");

            migrationBuilder.DropColumn(
                name: "IsCustom",
                table: "Cakes");

            migrationBuilder.DropColumn(
                name: "TextSize",
                table: "Cakes");

            migrationBuilder.DropColumn(
                name: "TextX",
                table: "Cakes");

            migrationBuilder.DropColumn(
                name: "TextY",
                table: "Cakes");

            migrationBuilder.RenameColumn(
                name: "Fillings",
                table: "Confectioners",
                newName: "Tastes");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "Cakes",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "Cakes",
                type: "text",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
