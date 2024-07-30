using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HealthMed.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    IdRole = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.IdRole);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    IdUser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdRole = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    CPF = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    CRM = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.IdUser);
                    table.ForeignKey(
                        name: "FK_users_roles_IdRole",
                        column: x => x.IdRole,
                        principalTable: "roles",
                        principalColumn: "IdRole");
                });

            migrationBuilder.CreateTable(
                name: "appointments",
                columns: table => new
                {
                    IdAppointment = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdDoctor = table.Column<int>(type: "int", nullable: false),
                    IdPatient = table.Column<int>(type: "int", nullable: false),
                    AppointmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HasBeenNotified = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appointments", x => x.IdAppointment);
                    table.ForeignKey(
                        name: "FK_appointments_users_IdDoctor",
                        column: x => x.IdDoctor,
                        principalTable: "users",
                        principalColumn: "IdUser");
                    table.ForeignKey(
                        name: "FK_appointments_users_IdPatient",
                        column: x => x.IdPatient,
                        principalTable: "users",
                        principalColumn: "IdUser");
                });

            migrationBuilder.CreateTable(
                name: "schedules",
                columns: table => new
                {
                    IdSchedule = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdDoctor = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_schedules", x => x.IdSchedule);
                    table.ForeignKey(
                        name: "FK_schedules_users_IdDoctor",
                        column: x => x.IdDoctor,
                        principalTable: "users",
                        principalColumn: "IdUser");
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "IdRole", "Name" },
                values: new object[,]
                {
                    { (byte)1, "Médico" },
                    { (byte)2, "Paciente" }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "IdUser", "CPF", "CRM", "CreatedAt", "IdRole", "LastUpdatedAt", "Name", "PasswordHash", "Email" },
                values: new object[,]
                {
                    { 10000, "16058582008", "1605858-BR", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), (byte)1, null, "Dr. Doctor 1", "MUKOsLOjfoh4YY1ZZLlp+CTyODjmgHhvPAp7PxFiCAWgXo1wibTbOrqht1UhnQi1", "dr.doctor1@healthmed.app" },
                    { 10001, "80084320044", "8008432-BR", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), (byte)1, null, "Dr. Doctor 2", "MUKOsLOjfoh4YY1ZZLlp+CTyODjmgHhvPAp7PxFiCAWgXo1wibTbOrqht1UhnQi1", "dr.doctor2@healthmed.app" },
                    { 10002, "04953908015", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), (byte)2, null, "Ailton", "LFhLAgFT8oinF3iXkk63ccZhEllpvGtr/OHG28On+hqniGeX+AIYe8UhNnqztEIm", "ailton@techchallenge.app" },
                    { 10003, "30050470086", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), (byte)2, null, "Bruno", "yobUq3aH9/R2x//xYdfaxqX2+FVBBLKzLipbFZILjsTo2sJ9cU/f2F4q6vvwIRzs", "bruno@techchallenge.app" },
                    { 10004, "80738939080", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), (byte)2, null, "Cecília", "LSHTSlFvEBDMS0tjoK2po682H7rLfgL2sXssgm/djzWWouzW4lIydGie7PbmX/1P", "cecilia@techchallenge.app" },
                    { 10005, "53900451060", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), (byte)2, null, "Cesar", "q1EyG7yB1S6Cwm7DGuDo3P8ZraEvVHTdBbKHZ1QW3TMG5JWVCnb3EO3UslYiiGeL", "cesar@techchallenge.app" },
                    { 10006, "41739149033", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), (byte)2, null, "Paulo", "XAro1VAlABuvkw5sxcSPEUdCeuTZRcM+9qLOumd79674Ro2V0bvvnlgb3zIkA7Yt", "paulo@techchallenge.app" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_appointments_IdDoctor",
                table: "appointments",
                column: "IdDoctor");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_IdPatient",
                table: "appointments",
                column: "IdPatient");

            migrationBuilder.CreateIndex(
                name: "IX_schedules_IdDoctor",
                table: "schedules",
                column: "IdDoctor");

            migrationBuilder.CreateIndex(
                name: "IX_users_IdRole",
                table: "users",
                column: "IdRole");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appointments");

            migrationBuilder.DropTable(
                name: "schedules");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
