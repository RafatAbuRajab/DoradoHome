using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoradoHome.Migrations
{
    /// <inheritdoc />
    public partial class tab : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSold",
                table: "Rentals");

            migrationBuilder.AddColumn<bool>(
                name: "IsRented",
                table: "Properties",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSold",
                table: "Properties",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRented",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "IsSold",
                table: "Properties");

            migrationBuilder.AddColumn<bool>(
                name: "IsSold",
                table: "Rentals",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
