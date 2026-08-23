using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatientService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Allergies",
                columns: table => new
                {
                    Code = table.Column<string>(type: "varchar(10)", nullable: false),
                    Name = table.Column<string>(type: "varchar(120)", nullable: false),
                    Description = table.Column<string>(type: "varchar(256)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ALLERGY", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "BloodTypes",
                columns: table => new
                {
                    Code = table.Column<string>(type: "varchar(10)", nullable: false),
                    Name = table.Column<string>(type: "varchar(120)", nullable: false),
                    Description = table.Column<string>(type: "varchar(256)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BLOOD_TYPE", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Conditions",
                columns: table => new
                {
                    Code = table.Column<string>(type: "varchar(10)", nullable: false),
                    Name = table.Column<string>(type: "varchar(120)", nullable: false),
                    Description = table.Column<string>(type: "varchar(256)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONDITION", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "ConsentTypes",
                columns: table => new
                {
                    Code = table.Column<string>(type: "varchar(10)", nullable: false),
                    Name = table.Column<string>(type: "varchar(120)", nullable: false),
                    Description = table.Column<string>(type: "varchar(256)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONSENT_TYPE", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Genders",
                columns: table => new
                {
                    Code = table.Column<string>(type: "varchar(10)", nullable: false),
                    Name = table.Column<string>(type: "varchar(120)", nullable: false),
                    Description = table.Column<string>(type: "varchar(256)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GENDER", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "InsurancePayers",
                columns: table => new
                {
                    Code = table.Column<string>(type: "varchar(10)", nullable: false),
                    Name = table.Column<string>(type: "varchar(120)", nullable: false),
                    Description = table.Column<string>(type: "varchar(256)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INSURANCE_PAYER", x => x.Code);
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
                name: "PatientStatuses",
                columns: table => new
                {
                    Code = table.Column<string>(type: "varchar(10)", nullable: false),
                    Name = table.Column<string>(type: "varchar(120)", nullable: false),
                    Description = table.Column<string>(type: "varchar(256)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PATIENT_STATUS", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "RelationshipTypes",
                columns: table => new
                {
                    Code = table.Column<string>(type: "varchar(10)", nullable: false),
                    Name = table.Column<string>(type: "varchar(120)", nullable: false),
                    Description = table.Column<string>(type: "varchar(256)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RELATIONSHIP_TYPE", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "varchar(100)", nullable: false),
                    LastName = table.Column<string>(type: "varchar(100)", nullable: false),
                    Email = table.Column<string>(type: "varchar(150)", nullable: false),
                    Phone = table.Column<string>(type: "varchar(50)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: false),
                    GenderCode = table.Column<string>(type: "varchar(10)", nullable: false),
                    BloodTypeCode = table.Column<string>(type: "varchar(10)", nullable: true),
                    NationalId = table.Column<string>(type: "varchar(20)", nullable: true),
                    CurrentStatusCode = table.Column<string>(type: "varchar(10)", nullable: false),
                    ProfilePictureUrl = table.Column<string>(type: "varchar(500)", nullable: true),
                    Notes = table.Column<string>(type: "varchar(2000)", nullable: true),
                    CurrentHealthInstitutionId = table.Column<int>(type: "int", nullable: true),
                    CurrentInstitutionName = table.Column<string>(type: "varchar(150)", nullable: true),
                    AssignedDoctorId = table.Column<Guid>(type: "uuid", nullable: true),
                    CurrentInsurancePayerCode = table.Column<string>(type: "varchar(10)", nullable: true),
                    CurrentInsurancePolicyNumber = table.Column<string>(type: "varchar(50)", nullable: true),
                    AddressLine = table.Column<string>(type: "varchar(250)", nullable: true),
                    SettlementId = table.Column<int>(type: "int", nullable: true),
                    MunicipalityId = table.Column<int>(type: "int", nullable: true),
                    CountryId = table.Column<int>(type: "int", nullable: true),
                    HeightCm = table.Column<decimal>(type: "numeric(5,1)", nullable: true),
                    WeightKg = table.Column<decimal>(type: "numeric(5,1)", nullable: true),
                    LastActive = table.Column<DateTime>(type: "timestamp", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PATIENT", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PATIENT_BLOOD_TYPE",
                        column: x => x.BloodTypeCode,
                        principalTable: "BloodTypes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PATIENT_GENDER",
                        column: x => x.GenderCode,
                        principalTable: "Genders",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PATIENT_INSURANCE_PAYER",
                        column: x => x.CurrentInsurancePayerCode,
                        principalTable: "InsurancePayers",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PATIENT_STATUS",
                        column: x => x.CurrentStatusCode,
                        principalTable: "PatientStatuses",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientAffiliationHistories",
                columns: table => new
                {
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    SequenceNumber = table.Column<decimal>(type: "numeric(2,0)", nullable: false),
                    HealthInstitutionId = table.Column<int>(type: "int", nullable: true),
                    InstitutionName = table.Column<string>(type: "varchar(150)", nullable: false),
                    From = table.Column<DateTime>(type: "timestamp", nullable: false),
                    To = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PATIENT_AFFILIATION_HISTORY", x => new { x.PatientId, x.SequenceNumber });
                    table.ForeignKey(
                        name: "FK_PATIENT_AFFILIATION_HISTORY_PATIENT",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientAllergyCoverages",
                columns: table => new
                {
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    AllergyCode = table.Column<string>(type: "varchar(10)", nullable: false),
                    Note = table.Column<string>(type: "varchar(500)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PATIENT_ALLERGY_COVERAGE", x => new { x.PatientId, x.AllergyCode });
                    table.ForeignKey(
                        name: "FK_PATIENT_ALLERGY_COVERAGE_ALLERGY",
                        column: x => x.AllergyCode,
                        principalTable: "Allergies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PATIENT_ALLERGY_COVERAGE_PATIENT",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientConditionCoverages",
                columns: table => new
                {
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConditionCode = table.Column<string>(type: "varchar(10)", nullable: false),
                    DiagnosedYear = table.Column<decimal>(type: "numeric(4,0)", nullable: true),
                    Note = table.Column<string>(type: "varchar(500)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PATIENT_CONDITION_COVERAGE", x => new { x.PatientId, x.ConditionCode });
                    table.ForeignKey(
                        name: "FK_PATIENT_CONDITION_COVERAGE_CONDITION",
                        column: x => x.ConditionCode,
                        principalTable: "Conditions",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PATIENT_CONDITION_COVERAGE_PATIENT",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientConsentCoverages",
                columns: table => new
                {
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConsentTypeCode = table.Column<string>(type: "varchar(10)", nullable: false),
                    From = table.Column<DateTime>(type: "timestamp", nullable: false),
                    To = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PATIENT_CONSENT_COVERAGE", x => new { x.PatientId, x.ConsentTypeCode });
                    table.ForeignKey(
                        name: "FK_PATIENT_CONSENT_COVERAGE_PATIENT",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PATIENT_CONSENT_COVERAGE_TYPE",
                        column: x => x.ConsentTypeCode,
                        principalTable: "ConsentTypes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientDoctorAssignmentHistories",
                columns: table => new
                {
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    SequenceNumber = table.Column<decimal>(type: "numeric(2,0)", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    From = table.Column<DateTime>(type: "timestamp", nullable: false),
                    To = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PATIENT_DOCTOR_ASSIGNMENT_HISTORY", x => new { x.PatientId, x.SequenceNumber });
                    table.ForeignKey(
                        name: "FK_PATIENT_DOCTOR_ASSIGNMENT_HISTORY_PATIENT",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientEmergencyContacts",
                columns: table => new
                {
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    SequenceNumber = table.Column<decimal>(type: "numeric(2,0)", nullable: false),
                    FullName = table.Column<string>(type: "varchar(150)", nullable: false),
                    Phone = table.Column<string>(type: "varchar(50)", nullable: false),
                    RelationshipCode = table.Column<string>(type: "varchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PATIENT_EMERGENCY_CONTACT", x => new { x.PatientId, x.SequenceNumber });
                    table.ForeignKey(
                        name: "FK_PATIENT_EMERGENCY_CONTACT_PATIENT",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PATIENT_EMERGENCY_CONTACT_RELATIONSHIP",
                        column: x => x.RelationshipCode,
                        principalTable: "RelationshipTypes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientInsuranceHistories",
                columns: table => new
                {
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    SequenceNumber = table.Column<decimal>(type: "numeric(2,0)", nullable: false),
                    PayerCode = table.Column<string>(type: "varchar(10)", nullable: false),
                    PolicyNumber = table.Column<string>(type: "varchar(50)", nullable: false),
                    From = table.Column<DateTime>(type: "timestamp", nullable: false),
                    To = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PATIENT_INSURANCE_HISTORY", x => new { x.PatientId, x.SequenceNumber });
                    table.ForeignKey(
                        name: "FK_PATIENT_INSURANCE_HISTORY_PATIENT",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PATIENT_INSURANCE_HISTORY_PAYER",
                        column: x => x.PayerCode,
                        principalTable: "InsurancePayers",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientLanguageCoverages",
                columns: table => new
                {
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    LanguageCode = table.Column<string>(type: "varchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PATIENT_LANGUAGE_COVERAGE", x => new { x.PatientId, x.LanguageCode });
                    table.ForeignKey(
                        name: "FK_PATIENT_LANGUAGE_COVERAGE_LANGUAGE",
                        column: x => x.LanguageCode,
                        principalTable: "Languages",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PATIENT_LANGUAGE_COVERAGE_PATIENT",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientStatusHistories",
                columns: table => new
                {
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    SequenceNumber = table.Column<decimal>(type: "numeric(2,0)", nullable: false),
                    StatusCode = table.Column<string>(type: "varchar(10)", nullable: false),
                    From = table.Column<DateTime>(type: "timestamp", nullable: false),
                    To = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PATIENT_STATUS_HISTORY", x => new { x.PatientId, x.SequenceNumber });
                    table.ForeignKey(
                        name: "FK_PATIENT_STATUS_HISTORY_PATIENT",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PATIENT_STATUS_HISTORY_STATUS",
                        column: x => x.StatusCode,
                        principalTable: "PatientStatuses",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PATIENT_AFFILIATION_HISTORY_PATIENT",
                table: "PatientAffiliationHistories",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PATIENT_ALLERGY_COVERAGE_ALLERGY",
                table: "PatientAllergyCoverages",
                column: "AllergyCode");

            migrationBuilder.CreateIndex(
                name: "IX_PATIENT_CONDITION_COVERAGE_CONDITION",
                table: "PatientConditionCoverages",
                column: "ConditionCode");

            migrationBuilder.CreateIndex(
                name: "IX_PATIENT_CONSENT_COVERAGE_TYPE",
                table: "PatientConsentCoverages",
                column: "ConsentTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_PATIENT_DOCTOR_ASSIGNMENT_HISTORY_DOCTOR",
                table: "PatientDoctorAssignmentHistories",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_PATIENT_DOCTOR_ASSIGNMENT_HISTORY_PATIENT",
                table: "PatientDoctorAssignmentHistories",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PATIENT_EMERGENCY_CONTACT_PATIENT",
                table: "PatientEmergencyContacts",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PATIENT_EMERGENCY_CONTACT_RELATIONSHIP",
                table: "PatientEmergencyContacts",
                column: "RelationshipCode");

            migrationBuilder.CreateIndex(
                name: "IX_PATIENT_INSURANCE_HISTORY_PATIENT",
                table: "PatientInsuranceHistories",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PATIENT_INSURANCE_HISTORY_PAYER",
                table: "PatientInsuranceHistories",
                column: "PayerCode");

            migrationBuilder.CreateIndex(
                name: "IX_PATIENT_LANGUAGE_COVERAGE_LANGUAGE",
                table: "PatientLanguageCoverages",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_PATIENT_ASSIGNED_DOCTOR",
                table: "Patients",
                column: "AssignedDoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_PATIENT_BLOOD_TYPE",
                table: "Patients",
                column: "BloodTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_PATIENT_GENDER",
                table: "Patients",
                column: "GenderCode");

            migrationBuilder.CreateIndex(
                name: "IX_PATIENT_LASTNAME",
                table: "Patients",
                column: "LastName");

            migrationBuilder.CreateIndex(
                name: "IX_PATIENT_STATUS",
                table: "Patients",
                column: "CurrentStatusCode");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_CurrentInsurancePayerCode",
                table: "Patients",
                column: "CurrentInsurancePayerCode");

            migrationBuilder.CreateIndex(
                name: "UX_PATIENT_EMAIL",
                table: "Patients",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_PATIENT_NATIONAL_ID",
                table: "Patients",
                column: "NationalId",
                unique: true,
                filter: "\"NationalId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PATIENT_STATUS_HISTORY_PATIENT",
                table: "PatientStatusHistories",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PATIENT_STATUS_HISTORY_STATUS",
                table: "PatientStatusHistories",
                column: "StatusCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientAffiliationHistories");

            migrationBuilder.DropTable(
                name: "PatientAllergyCoverages");

            migrationBuilder.DropTable(
                name: "PatientConditionCoverages");

            migrationBuilder.DropTable(
                name: "PatientConsentCoverages");

            migrationBuilder.DropTable(
                name: "PatientDoctorAssignmentHistories");

            migrationBuilder.DropTable(
                name: "PatientEmergencyContacts");

            migrationBuilder.DropTable(
                name: "PatientInsuranceHistories");

            migrationBuilder.DropTable(
                name: "PatientLanguageCoverages");

            migrationBuilder.DropTable(
                name: "PatientStatusHistories");

            migrationBuilder.DropTable(
                name: "Allergies");

            migrationBuilder.DropTable(
                name: "Conditions");

            migrationBuilder.DropTable(
                name: "ConsentTypes");

            migrationBuilder.DropTable(
                name: "RelationshipTypes");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "BloodTypes");

            migrationBuilder.DropTable(
                name: "Genders");

            migrationBuilder.DropTable(
                name: "InsurancePayers");

            migrationBuilder.DropTable(
                name: "PatientStatuses");
        }
    }
}
