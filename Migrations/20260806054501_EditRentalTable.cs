using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoradoHome.Migrations
{
    /// <inheritdoc />
    public partial class EditRentalTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRented",
                table: "Rentals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSold",
                table: "Rentals",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRented",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "IsSold",
                table: "Rentals");
        }
    }
}
