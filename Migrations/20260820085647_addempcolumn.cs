using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoradoHome.Migrations
{
    /// <inheritdoc />
    public partial class addempcolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "Offers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Offers_EmployeeId",
                table: "Offers",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Employees_EmployeeId",
                table: "Offers",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Employees_EmployeeId",
                table: "Offers");

            migrationBuilder.DropIndex(
                name: "IX_Offers_EmployeeId",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Offers");
        }
    }
}
