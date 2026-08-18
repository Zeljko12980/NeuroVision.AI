using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PdfService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePdfTemplateTable1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SignatureHeight",
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

            migrationBuilder.AlterColumn<int>(
                name: "SignaturePage",
                table: "pdf_templates",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<bool>(
                name: "RequiresSignature",
                table: "pdf_templates",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.CreateTable(
                name: "PdfTemplateFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PdfTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Page = table.Column<int>(type: "integer", nullable: false),
                    X = table.Column<float>(type: "real", nullable: false),
                    Y = table.Column<float>(type: "real", nullable: false),
                    Width = table.Column<float>(type: "real", nullable: false),
                    Height = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PdfTemplateFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PdfTemplateFields_pdf_templates_PdfTemplateId",
                        column: x => x.PdfTemplateId,
                        principalTable: "pdf_templates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PdfTemplateFields_PdfTemplateId",
                table: "PdfTemplateFields",
                column: "PdfTemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PdfTemplateFields");

            migrationBuilder.AlterColumn<int>(
                name: "SignaturePage",
                table: "pdf_templates",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<bool>(
                name: "RequiresSignature",
                table: "pdf_templates",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<float>(
                name: "SignatureHeight",
                table: "pdf_templates",
                type: "real",
                nullable: false,
                defaultValue: 0f);

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
    }
}
