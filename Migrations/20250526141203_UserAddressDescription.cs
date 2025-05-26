using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Caker.Migrations
{
    /// <inheritdoc />
    public partial class UserAddressDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Confectioners");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Confectioners");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Confectioners",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Confectioners",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
