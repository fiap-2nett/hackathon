using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HealthMed.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_AppointmentStatus_Entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IdPatient",
                table: "appointments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "CanceledAt",
                table: "appointments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "IdAppointmentStatus",
                table: "appointments",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateTable(
                name: "appointmentstatus",
                columns: table => new
                {
                    IdAppointmentStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appointmentstatus", x => x.IdAppointmentStatus);
                });

            migrationBuilder.InsertData(
                table: "appointmentstatus",
                columns: new[] { "IdAppointmentStatus", "Name" },
                values: new object[,]
                {
                    { (byte)1, "Disponível" },
                    { (byte)2, "Ocupado" },
                    { (byte)3, "Cancelado" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_appointments_IdAppointmentStatus",
                table: "appointments",
                column: "IdAppointmentStatus");

            migrationBuilder.AddForeignKey(
                name: "FK_appointments_appointmentstatus_IdAppointmentStatus",
                table: "appointments",
                column: "IdAppointmentStatus",
                principalTable: "appointmentstatus",
                principalColumn: "IdAppointmentStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointments_appointmentstatus_IdAppointmentStatus",
                table: "appointments");

            migrationBuilder.DropTable(
                name: "appointmentstatus");

            migrationBuilder.DropIndex(
                name: "IX_appointments_IdAppointmentStatus",
                table: "appointments");

            migrationBuilder.DropColumn(
                name: "CanceledAt",
                table: "appointments");

            migrationBuilder.DropColumn(
                name: "IdAppointmentStatus",
                table: "appointments");

            migrationBuilder.AlterColumn<int>(
                name: "IdPatient",
                table: "appointments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
