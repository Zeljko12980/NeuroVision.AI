using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TumorDetectionService.Infrastructure.Persistence;

#nullable disable

namespace TumorDetectionService.Infrastructure.Persistence.Migrations;

[DbContext(typeof(TumorDetectionDbContext))]
[Migration("20260825220000_AddClinicalFollowUp")]
public partial class AddClinicalFollowUp : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "clinical_catalog_items",
            columns: table => new
            {
                Category = table.Column<int>(type: "integer", nullable: false),
                Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_clinical_catalog_items", x => new { x.Category, x.Code });
            });

        migrationBuilder.CreateTable(
            name: "analysis_clinical_follow_ups",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TumorAnalysisId = table.Column<Guid>(type: "uuid", nullable: false),
                GradeCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                OperabilityCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                SpreadCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                TreatmentOptionCodes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                SizeLocationNotes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                ClinicalNotes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_analysis_clinical_follow_ups", x => x.Id);
                table.ForeignKey(
                    name: "FK_analysis_clinical_follow_ups_tumor_analyses_TumorAnalysisId",
                    column: x => x.TumorAnalysisId,
                    principalTable: "tumor_analyses",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_analysis_clinical_follow_ups_TumorAnalysisId",
            table: "analysis_clinical_follow_ups",
            column: "TumorAnalysisId",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "analysis_clinical_follow_ups");
        migrationBuilder.DropTable(name: "clinical_catalog_items");
    }
}
