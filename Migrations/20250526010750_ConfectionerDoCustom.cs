using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Caker.Migrations
{
    /// <inheritdoc />
    public partial class ConfectionerDoCustom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DoCustom",
                table: "Confectioners",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoCustom",
                table: "Confectioners");
        }
    }
}
