using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TumorDetectionService.Infrastructure.Persistence;

#nullable disable

namespace TumorDetectionService.Infrastructure.Persistence.Migrations;

[DbContext(typeof(TumorDetectionDbContext))]
[Migration("20260813180000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ai_model_versions",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TaskType = table.Column<int>(type: "integer", nullable: false),
                VersionLabel = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                RunId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                WeightsPath = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                RegisteredByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                RegisteredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_ai_model_versions", x => x.Id));

        migrationBuilder.CreateTable(
            name: "brain_scans",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                UploadedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                FileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                StoredFilePath = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                ScanType = table.Column<int>(type: "integer", nullable: false),
                FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_brain_scans", x => x.Id));

        migrationBuilder.CreateTable(
            name: "tumor_analyses",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                BrainScanId = table.Column<Guid>(type: "uuid", nullable: false),
                RequestedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                RequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                DetectionRunId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                ClassificationRunId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                SegmentationRunId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                ReportFilePath = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                OverallConfidence = table.Column<double>(type: "double precision", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_tumor_analyses", x => x.Id);
                table.ForeignKey(
                    name: "FK_tumor_analyses_brain_scans_BrainScanId",
                    column: x => x.BrainScanId,
                    principalTable: "brain_scans",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "analysis_comments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TumorAnalysisId = table.Column<Guid>(type: "uuid", nullable: false),
                AuthorUserId = table.Column<Guid>(type: "uuid", nullable: false),
                Content = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_analysis_comments", x => x.Id);
                table.ForeignKey(
                    name: "FK_analysis_comments_tumor_analyses_TumorAnalysisId",
                    column: x => x.TumorAnalysisId,
                    principalTable: "tumor_analyses",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "analysis_error_logs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TumorAnalysisId = table.Column<Guid>(type: "uuid", nullable: true),
                Message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                Details = table.Column<string>(type: "text", nullable: true),
                OccurredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_analysis_error_logs", x => x.Id);
                table.ForeignKey(
                    name: "FK_analysis_error_logs_tumor_analyses_TumorAnalysisId",
                    column: x => x.TumorAnalysisId,
                    principalTable: "tumor_analyses",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateTable(
            name: "classification_results",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TumorAnalysisId = table.Column<Guid>(type: "uuid", nullable: false),
                PredictedClass = table.Column<int>(type: "integer", nullable: false),
                Confidence = table.Column<double>(type: "double precision", nullable: false),
                ProbabilitiesJson = table.Column<string>(type: "jsonb", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_classification_results", x => x.Id);
                table.ForeignKey(
                    name: "FK_classification_results_tumor_analyses_TumorAnalysisId",
                    column: x => x.TumorAnalysisId,
                    principalTable: "tumor_analyses",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "detection_findings",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TumorAnalysisId = table.Column<Guid>(type: "uuid", nullable: false),
                ClassName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Confidence = table.Column<double>(type: "double precision", nullable: false),
                XCenter = table.Column<double>(type: "double precision", nullable: false),
                YCenter = table.Column<double>(type: "double precision", nullable: false),
                Width = table.Column<double>(type: "double precision", nullable: false),
                Height = table.Column<double>(type: "double precision", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_detection_findings", x => x.Id);
                table.ForeignKey(
                    name: "FK_detection_findings_tumor_analyses_TumorAnalysisId",
                    column: x => x.TumorAnalysisId,
                    principalTable: "tumor_analyses",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "manual_corrections",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TumorAnalysisId = table.Column<Guid>(type: "uuid", nullable: false),
                CorrectedClass = table.Column<int>(type: "integer", nullable: false),
                Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                CorrectedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                CorrectedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_manual_corrections", x => x.Id);
                table.ForeignKey(
                    name: "FK_manual_corrections_tumor_analyses_TumorAnalysisId",
                    column: x => x.TumorAnalysisId,
                    principalTable: "tumor_analyses",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "segmentation_results",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TumorAnalysisId = table.Column<Guid>(type: "uuid", nullable: false),
                TumorAreaRatio = table.Column<double>(type: "double precision", nullable: false),
                MaskFilePath = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                AnnotatedImagePath = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_segmentation_results", x => x.Id);
                table.ForeignKey(
                    name: "FK_segmentation_results_tumor_analyses_TumorAnalysisId",
                    column: x => x.TumorAnalysisId,
                    principalTable: "tumor_analyses",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(name: "IX_ai_model_versions_RunId", table: "ai_model_versions", column: "RunId", unique: true);
        migrationBuilder.CreateIndex(name: "IX_ai_model_versions_TaskType_IsActive", table: "ai_model_versions", columns: new[] { "TaskType", "IsActive" });
        migrationBuilder.CreateIndex(name: "IX_analysis_comments_TumorAnalysisId", table: "analysis_comments", column: "TumorAnalysisId");
        migrationBuilder.CreateIndex(name: "IX_analysis_error_logs_OccurredAt", table: "analysis_error_logs", column: "OccurredAt");
        migrationBuilder.CreateIndex(name: "IX_analysis_error_logs_TumorAnalysisId", table: "analysis_error_logs", column: "TumorAnalysisId");
        migrationBuilder.CreateIndex(name: "IX_brain_scans_PatientId", table: "brain_scans", column: "PatientId");
        migrationBuilder.CreateIndex(name: "IX_brain_scans_UploadedAt", table: "brain_scans", column: "UploadedAt");
        migrationBuilder.CreateIndex(name: "IX_classification_results_TumorAnalysisId", table: "classification_results", column: "TumorAnalysisId", unique: true);
        migrationBuilder.CreateIndex(name: "IX_detection_findings_TumorAnalysisId", table: "detection_findings", column: "TumorAnalysisId");
        migrationBuilder.CreateIndex(name: "IX_manual_corrections_TumorAnalysisId", table: "manual_corrections", column: "TumorAnalysisId", unique: true);
        migrationBuilder.CreateIndex(name: "IX_segmentation_results_TumorAnalysisId", table: "segmentation_results", column: "TumorAnalysisId", unique: true);
        migrationBuilder.CreateIndex(name: "IX_tumor_analyses_BrainScanId", table: "tumor_analyses", column: "BrainScanId");
        migrationBuilder.CreateIndex(name: "IX_tumor_analyses_RequestedAt", table: "tumor_analyses", column: "RequestedAt");
        migrationBuilder.CreateIndex(name: "IX_tumor_analyses_Status", table: "tumor_analyses", column: "Status");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "ai_model_versions");
        migrationBuilder.DropTable(name: "analysis_comments");
        migrationBuilder.DropTable(name: "analysis_error_logs");
        migrationBuilder.DropTable(name: "classification_results");
        migrationBuilder.DropTable(name: "detection_findings");
        migrationBuilder.DropTable(name: "manual_corrections");
        migrationBuilder.DropTable(name: "segmentation_results");
        migrationBuilder.DropTable(name: "tumor_analyses");
        migrationBuilder.DropTable(name: "brain_scans");
    }
}
