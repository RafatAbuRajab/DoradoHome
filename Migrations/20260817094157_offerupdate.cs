using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoradoHome.Migrations
{
    /// <inheritdoc />
    public partial class offerupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Clients_ClientId",
                table: "Offers");

            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Properties_PropertyId",
                table: "Offers");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyId",
                table: "Offers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ClientId",
                table: "Offers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AppointmentId",
                table: "Offers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Offers_AppointmentId",
                table: "Offers",
                column: "AppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Appointments_AppointmentId",
                table: "Offers",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Clients_ClientId",
                table: "Offers",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Properties_PropertyId",
                table: "Offers",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Appointments_AppointmentId",
                table: "Offers");

            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Clients_ClientId",
                table: "Offers");

            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Properties_PropertyId",
                table: "Offers");

            migrationBuilder.DropIndex(
                name: "IX_Offers_AppointmentId",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "AppointmentId",
                table: "Offers");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyId",
                table: "Offers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ClientId",
                table: "Offers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Clients_ClientId",
                table: "Offers",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Properties_PropertyId",
                table: "Offers",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
