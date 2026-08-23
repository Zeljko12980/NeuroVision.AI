using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointmentService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppointmentStatuses",
                columns: table => new
                {
                    Code = table.Column<string>(type: "varchar(10)", nullable: false),
                    Name = table.Column<string>(type: "varchar(120)", nullable: false),
                    Description = table.Column<string>(type: "varchar(256)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APPOINTMENT_STATUS", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "AppointmentTypes",
                columns: table => new
                {
                    Code = table.Column<string>(type: "varchar(10)", nullable: false),
                    Name = table.Column<string>(type: "varchar(120)", nullable: false),
                    Description = table.Column<string>(type: "varchar(256)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APPOINTMENT_TYPE", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    TypeCode = table.Column<string>(type: "varchar(10)", nullable: false),
                    StatusCode = table.Column<string>(type: "varchar(10)", nullable: false),
                    StartsAt = table.Column<DateTime>(type: "timestamp", nullable: false),
                    EndsAt = table.Column<DateTime>(type: "timestamp", nullable: false),
                    Title = table.Column<string>(type: "varchar(120)", nullable: false),
                    Notes = table.Column<string>(type: "varchar(512)", nullable: true),
                    HealthInstitutionId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false),
                    CancelledAt = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APPOINTMENT", x => x.Id);
                    table.ForeignKey(
                        name: "FK_APPOINTMENT_STATUS",
                        column: x => x.StatusCode,
                        principalTable: "AppointmentStatuses",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_APPOINTMENT_TYPE",
                        column: x => x.TypeCode,
                        principalTable: "AppointmentTypes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_APPOINTMENT_DOCTOR_START",
                table: "Appointments",
                columns: new[] { "DoctorId", "StartsAt" });

            migrationBuilder.CreateIndex(
                name: "IX_APPOINTMENT_PATIENT_START",
                table: "Appointments",
                columns: new[] { "PatientId", "StartsAt" });

            migrationBuilder.CreateIndex(
                name: "IX_APPOINTMENT_STATUS",
                table: "Appointments",
                column: "StatusCode");

            migrationBuilder.CreateIndex(
                name: "IX_APPOINTMENT_TYPE",
                table: "Appointments",
                column: "TypeCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "AppointmentStatuses");

            migrationBuilder.DropTable(
                name: "AppointmentTypes");
        }
    }
}
