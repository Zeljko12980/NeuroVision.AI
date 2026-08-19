using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PdfService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCertificateUserAndSignature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Certificates",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SignatureImagePath",
                table: "Certificates",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_UserId",
                table: "Certificates",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Certificates_UserId",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "SignatureImagePath",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Certificates");
        }
    }
}
