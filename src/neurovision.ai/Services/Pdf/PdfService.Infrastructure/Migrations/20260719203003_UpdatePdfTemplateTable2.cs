using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PdfService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePdfTemplateTable2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PdfTemplateFields_pdf_templates_PdfTemplateId",
                table: "PdfTemplateFields");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PdfTemplateFields",
                table: "PdfTemplateFields");

            migrationBuilder.RenameTable(
                name: "PdfTemplateFields",
                newName: "pdf_template_fields");

            migrationBuilder.RenameIndex(
                name: "IX_PdfTemplateFields_PdfTemplateId",
                table: "pdf_template_fields",
                newName: "IX_pdf_template_fields_PdfTemplateId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_pdf_template_fields",
                table: "pdf_template_fields",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_pdf_template_fields_pdf_templates_PdfTemplateId",
                table: "pdf_template_fields",
                column: "PdfTemplateId",
                principalTable: "pdf_templates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pdf_template_fields_pdf_templates_PdfTemplateId",
                table: "pdf_template_fields");

            migrationBuilder.DropPrimaryKey(
                name: "PK_pdf_template_fields",
                table: "pdf_template_fields");

            migrationBuilder.RenameTable(
                name: "pdf_template_fields",
                newName: "PdfTemplateFields");

            migrationBuilder.RenameIndex(
                name: "IX_pdf_template_fields_PdfTemplateId",
                table: "PdfTemplateFields",
                newName: "IX_PdfTemplateFields_PdfTemplateId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PdfTemplateFields",
                table: "PdfTemplateFields",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PdfTemplateFields_pdf_templates_PdfTemplateId",
                table: "PdfTemplateFields",
                column: "PdfTemplateId",
                principalTable: "pdf_templates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
