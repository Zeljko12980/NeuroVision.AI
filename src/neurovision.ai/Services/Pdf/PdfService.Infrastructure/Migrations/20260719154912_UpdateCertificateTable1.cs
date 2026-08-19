using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PdfService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCertificateTable1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProtectedPassword",
                table: "Certificates",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProtectedPassword",
                table: "Certificates");
        }
    }
}
