using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoctorService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DegreeTypes",
                columns: table => new
                {
                    Code = table.Column<string>(type: "varchar(10)", nullable: false),
                    Name = table.Column<string>(type: "varchar(120)", nullable: false),
                    Description = table.Column<string>(type: "varchar(256)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DEGREE_TYPE", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "DoctorStatuses",
                columns: table => new
                {
                    Code = table.Column<string>(type: "varchar(10)", nullable: false),
                    Name = table.Column<string>(type: "varchar(120)", nullable: false),
                    Description = table.Column<string>(type: "varchar(256)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOCTOR_STATUS", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Code = table.Column<string>(type: "varchar(10)", nullable: false),
                    Name = table.Column<string>(type: "varchar(120)", nullable: false),
                    Description = table.Column<string>(type: "varchar(256)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LANGUAGE", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "LicenseAuthorities",
                columns: table => new
                {
                    Code = table.Column<string>(type: "varchar(10)", nullable: false),
                    Name = table.Column<string>(type: "varchar(120)", nullable: false),
                    Description = table.Column<string>(type: "varchar(256)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LICENSE_AUTHORITY", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Specializations",
                columns: table => new
                {
                    Code = table.Column<string>(type: "varchar(10)", nullable: false),
                    Name = table.Column<string>(type: "varchar(120)", nullable: false),
                    Description = table.Column<string>(type: "varchar(256)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPECIALIZATION", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Doctors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "varchar(100)", nullable: false),
                    LastName = table.Column<string>(type: "varchar(100)", nullable: false),
                    Email = table.Column<string>(type: "varchar(150)", nullable: false),
                    Phone = table.Column<string>(type: "varchar(50)", nullable: false),
                    LicenseNumber = table.Column<string>(type: "varchar(50)", nullable: false),
                    LicenseAuthorityCode = table.Column<string>(type: "varchar(10)", nullable: true),
                    CurrentSpecializationCode = table.Column<string>(type: "varchar(10)", nullable: false),
                    CurrentStatusCode = table.Column<string>(type: "varchar(10)", nullable: false),
                    ProfilePictureUrl = table.Column<string>(type: "varchar(500)", nullable: true),
                    Bio = table.Column<string>(type: "varchar(2000)", nullable: true),
                    CurrentHealthInstitutionId = table.Column<int>(type: "int", nullable: true),
                    CurrentInstitutionName = table.Column<string>(type: "varchar(150)", nullable: true),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    LastActive = table.Column<DateTime>(type: "timestamp", nullable: false),
                    AverageRating = table.Column<decimal>(type: "numeric(3,2)", nullable: false, defaultValue: 0m),
                    TotalReviews = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOCTOR", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DOCTOR_LICENSE_AUTHORITY",
                        column: x => x.LicenseAuthorityCode,
                        principalTable: "LicenseAuthorities",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DOCTOR_SPECIALIZATION",
                        column: x => x.CurrentSpecializationCode,
                        principalTable: "Specializations",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DOCTOR_STATUS",
                        column: x => x.CurrentStatusCode,
                        principalTable: "DoctorStatuses",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DoctorAffiliationHistories",
                columns: table => new
                {
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    SequenceNumber = table.Column<decimal>(type: "numeric(2,0)", nullable: false),
                    HealthInstitutionId = table.Column<int>(type: "int", nullable: true),
                    InstitutionName = table.Column<string>(type: "varchar(150)", nullable: false),
                    From = table.Column<DateTime>(type: "timestamp", nullable: false),
                    To = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOCTOR_AFFILIATION_HISTORY", x => new { x.DoctorId, x.SequenceNumber });
                    table.ForeignKey(
                        name: "FK_DOCTOR_AFFILIATION_HISTORY_DOCTOR",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DoctorDegreeCoverages",
                columns: table => new
                {
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    DegreeTypeCode = table.Column<string>(type: "varchar(10)", nullable: false),
                    InstitutionName = table.Column<string>(type: "varchar(150)", nullable: true),
                    Year = table.Column<decimal>(type: "numeric(4,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOCTOR_DEGREE_COVERAGE", x => new { x.DoctorId, x.DegreeTypeCode });
                    table.ForeignKey(
                        name: "FK_DOCTOR_DEGREE_COVERAGE_DEGREE_TYPE",
                        column: x => x.DegreeTypeCode,
                        principalTable: "DegreeTypes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DOCTOR_DEGREE_COVERAGE_DOCTOR",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DoctorLanguageCoverages",
                columns: table => new
                {
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    LanguageCode = table.Column<string>(type: "varchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOCTOR_LANGUAGE_COVERAGE", x => new { x.DoctorId, x.LanguageCode });
                    table.ForeignKey(
                        name: "FK_DOCTOR_LANGUAGE_COVERAGE_DOCTOR",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DOCTOR_LANGUAGE_COVERAGE_LANGUAGE",
                        column: x => x.LanguageCode,
                        principalTable: "Languages",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DoctorLicenseHistories",
                columns: table => new
                {
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    SequenceNumber = table.Column<decimal>(type: "numeric(2,0)", nullable: false),
                    LicenseNumber = table.Column<string>(type: "varchar(50)", nullable: false),
                    LicenseAuthorityCode = table.Column<string>(type: "varchar(10)", nullable: true),
                    From = table.Column<DateTime>(type: "timestamp", nullable: false),
                    To = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOCTOR_LICENSE_HISTORY", x => new { x.DoctorId, x.SequenceNumber });
                    table.ForeignKey(
                        name: "FK_DOCTOR_LICENSE_HISTORY_AUTHORITY",
                        column: x => x.LicenseAuthorityCode,
                        principalTable: "LicenseAuthorities",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DOCTOR_LICENSE_HISTORY_DOCTOR",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DoctorReviews",
                columns: table => new
                {
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    SequenceNumber = table.Column<decimal>(type: "numeric(2,0)", nullable: false),
                    Rating = table.Column<decimal>(type: "numeric(2,1)", nullable: false),
                    Comment = table.Column<string>(type: "varchar(2000)", nullable: true),
                    ReviewerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOCTOR_REVIEW", x => new { x.DoctorId, x.SequenceNumber });
                    table.ForeignKey(
                        name: "FK_DOCTOR_REVIEW_DOCTOR",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DoctorSpecializationCoverages",
                columns: table => new
                {
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpecializationCode = table.Column<string>(type: "varchar(10)", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    From = table.Column<DateTime>(type: "timestamp", nullable: false),
                    To = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOCTOR_SPECIALIZATION_COVERAGE", x => new { x.DoctorId, x.SpecializationCode });
                    table.ForeignKey(
                        name: "FK_DOCTOR_SPECIALIZATION_COVERAGE_DOCTOR",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DOCTOR_SPECIALIZATION_COVERAGE_SPECIALIZATION",
                        column: x => x.SpecializationCode,
                        principalTable: "Specializations",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DoctorStatusHistories",
                columns: table => new
                {
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    SequenceNumber = table.Column<decimal>(type: "numeric(2,0)", nullable: false),
                    StatusCode = table.Column<string>(type: "varchar(10)", nullable: false),
                    From = table.Column<DateTime>(type: "timestamp", nullable: false),
                    To = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOCTOR_STATUS_HISTORY", x => new { x.DoctorId, x.SequenceNumber });
                    table.ForeignKey(
                        name: "FK_DOCTOR_STATUS_HISTORY_DOCTOR",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DOCTOR_STATUS_HISTORY_STATUS",
                        column: x => x.StatusCode,
                        principalTable: "DoctorStatuses",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkingSlots",
                columns: table => new
                {
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    DayOfWeek = table.Column<short>(type: "smallint", nullable: false),
                    SequenceNumber = table.Column<decimal>(type: "numeric(2,0)", nullable: false),
                    Start = table.Column<TimeSpan>(type: "time", nullable: false),
                    End = table.Column<TimeSpan>(type: "time", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WORKING_SLOT", x => new { x.DoctorId, x.DayOfWeek, x.SequenceNumber });
                    table.ForeignKey(
                        name: "FK_WORKING_SLOT_DOCTOR",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DOCTOR_AFFILIATION_HISTORY_DOCTOR",
                table: "DoctorAffiliationHistories",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_DOCTOR_DEGREE_COVERAGE_DEGREE_TYPE",
                table: "DoctorDegreeCoverages",
                column: "DegreeTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_DOCTOR_LANGUAGE_COVERAGE_LANGUAGE",
                table: "DoctorLanguageCoverages",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_DOCTOR_LICENSE_HISTORY_AUTHORITY",
                table: "DoctorLicenseHistories",
                column: "LicenseAuthorityCode");

            migrationBuilder.CreateIndex(
                name: "IX_DOCTOR_LICENSE_HISTORY_DOCTOR",
                table: "DoctorLicenseHistories",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_DOCTOR_REVIEW_DOCTOR",
                table: "DoctorReviews",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_DOCTOR_AVAILABLE_STATUS",
                table: "Doctors",
                columns: new[] { "IsAvailable", "CurrentStatusCode" });

            migrationBuilder.CreateIndex(
                name: "IX_DOCTOR_LASTNAME",
                table: "Doctors",
                column: "LastName");

            migrationBuilder.CreateIndex(
                name: "IX_DOCTOR_LICENSE_AUTHORITY",
                table: "Doctors",
                column: "LicenseAuthorityCode");

            migrationBuilder.CreateIndex(
                name: "IX_DOCTOR_SPECIALIZATION",
                table: "Doctors",
                column: "CurrentSpecializationCode");

            migrationBuilder.CreateIndex(
                name: "IX_DOCTOR_STATUS",
                table: "Doctors",
                column: "CurrentStatusCode");

            migrationBuilder.CreateIndex(
                name: "UX_DOCTOR_EMAIL",
                table: "Doctors",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_DOCTOR_LICENSE",
                table: "Doctors",
                column: "LicenseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DOCTOR_SPECIALIZATION_COVERAGE_SPECIALIZATION",
                table: "DoctorSpecializationCoverages",
                column: "SpecializationCode");

            migrationBuilder.CreateIndex(
                name: "IX_DOCTOR_STATUS_HISTORY_DOCTOR",
                table: "DoctorStatusHistories",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_DOCTOR_STATUS_HISTORY_STATUS",
                table: "DoctorStatusHistories",
                column: "StatusCode");

            migrationBuilder.CreateIndex(
                name: "IX_WORKING_SLOT_DOCTOR",
                table: "WorkingSlots",
                column: "DoctorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DoctorAffiliationHistories");

            migrationBuilder.DropTable(
                name: "DoctorDegreeCoverages");

            migrationBuilder.DropTable(
                name: "DoctorLanguageCoverages");

            migrationBuilder.DropTable(
                name: "DoctorLicenseHistories");

            migrationBuilder.DropTable(
                name: "DoctorReviews");

            migrationBuilder.DropTable(
                name: "DoctorSpecializationCoverages");

            migrationBuilder.DropTable(
                name: "DoctorStatusHistories");

            migrationBuilder.DropTable(
                name: "WorkingSlots");

            migrationBuilder.DropTable(
                name: "DegreeTypes");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "Doctors");

            migrationBuilder.DropTable(
                name: "LicenseAuthorities");

            migrationBuilder.DropTable(
                name: "Specializations");

            migrationBuilder.DropTable(
                name: "DoctorStatuses");
        }
    }
}
