using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Caker.Migrations
{
    /// <inheritdoc />
    public partial class DiametersTastes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "MaxDiameter",
                table: "Confectioners",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MinDiameter",
                table: "Confectioners",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string[]>(
                name: "Tastes",
                table: "Confectioners",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Taste",
                table: "Cakes",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumber",
                table: "Users",
                column: "PhoneNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_PhoneNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MaxDiameter",
                table: "Confectioners");

            migrationBuilder.DropColumn(
                name: "MinDiameter",
                table: "Confectioners");

            migrationBuilder.DropColumn(
                name: "Tastes",
                table: "Confectioners");

            migrationBuilder.DropColumn(
                name: "Taste",
                table: "Cakes");
        }
    }
}
