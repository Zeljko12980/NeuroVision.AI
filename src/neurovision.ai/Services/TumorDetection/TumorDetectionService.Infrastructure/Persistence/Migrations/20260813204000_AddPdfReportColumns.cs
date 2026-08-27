using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TumorDetectionService.Infrastructure.Persistence;

#nullable disable

namespace TumorDetectionService.Infrastructure.Persistence.Migrations;

[DbContext(typeof(TumorDetectionDbContext))]
[Migration("20260813204000_AddPdfReportColumns")]
public partial class AddPdfReportColumns : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "PdfReportPath",
            table: "tumor_analyses",
            type: "character varying(1000)",
            maxLength: 1000,
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "PdfGeneratedAt",
            table: "tumor_analyses",
            type: "timestamp with time zone",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "PdfReportPath", table: "tumor_analyses");
        migrationBuilder.DropColumn(name: "PdfGeneratedAt", table: "tumor_analyses");
    }
}
