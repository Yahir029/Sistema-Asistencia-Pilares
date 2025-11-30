using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AsistenciaAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailTelefonoToEmpleado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Empleados",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Empleados",
                type: "TEXT",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Empleados");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Empleados");
        }
    }
}
