using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthMed.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_OptimisticLocking_In_Appointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "appointments",
                type: "rowversion",
                rowVersion: true,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "appointments");
        }
    }
}
