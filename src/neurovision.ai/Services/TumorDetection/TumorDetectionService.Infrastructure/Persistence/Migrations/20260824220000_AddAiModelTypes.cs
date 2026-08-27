using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TumorDetectionService.Infrastructure.Persistence;

#nullable disable

namespace TumorDetectionService.Infrastructure.Persistence.Migrations;

[DbContext(typeof(TumorDetectionDbContext))]
[Migration("20260824220000_AddAiModelTypes")]
public partial class AddAiModelTypes : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ai_model_types",
            columns: table => new
            {
                Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_ai_model_types", x => x.Code));
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "ai_model_types");
    }
}
