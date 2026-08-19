using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PdfService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePdfTemplateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RequiresSignature",
                table: "pdf_templates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<float>(
                name: "SignatureHeight",
                table: "pdf_templates",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "SignaturePage",
                table: "pdf_templates",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "SignatureWidth",
                table: "pdf_templates",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "SignatureX",
                table: "pdf_templates",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "SignatureY",
                table: "pdf_templates",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiresSignature",
                table: "pdf_templates");

            migrationBuilder.DropColumn(
                name: "SignatureHeight",
                table: "pdf_templates");

            migrationBuilder.DropColumn(
                name: "SignaturePage",
                table: "pdf_templates");

            migrationBuilder.DropColumn(
                name: "SignatureWidth",
                table: "pdf_templates");

            migrationBuilder.DropColumn(
                name: "SignatureX",
                table: "pdf_templates");

            migrationBuilder.DropColumn(
                name: "SignatureY",
                table: "pdf_templates");
        }
    }
}
