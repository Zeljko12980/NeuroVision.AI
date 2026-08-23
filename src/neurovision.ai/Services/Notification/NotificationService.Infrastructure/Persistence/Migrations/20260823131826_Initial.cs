using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotificationService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificationChannels",
                columns: table => new
                {
                    Code = table.Column<string>(type: "varchar(10)", nullable: false),
                    Name = table.Column<string>(type: "varchar(120)", nullable: false),
                    Description = table.Column<string>(type: "varchar(256)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NOTIFICATION_CHANNEL", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "NotificationSeverities",
                columns: table => new
                {
                    Code = table.Column<string>(type: "varchar(10)", nullable: false),
                    Name = table.Column<string>(type: "varchar(120)", nullable: false),
                    Description = table.Column<string>(type: "varchar(256)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NOTIFICATION_SEVERITY", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "NotificationTypes",
                columns: table => new
                {
                    Code = table.Column<string>(type: "varchar(10)", nullable: false),
                    Name = table.Column<string>(type: "varchar(120)", nullable: false),
                    Description = table.Column<string>(type: "varchar(256)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NOTIFICATION_TYPE", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "NotificationPreferences",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TypeCode = table.Column<string>(type: "varchar(10)", nullable: false),
                    ChannelCode = table.Column<string>(type: "varchar(10)", nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NOTIFICATION_PREFERENCE", x => new { x.UserId, x.TypeCode, x.ChannelCode });
                    table.ForeignKey(
                        name: "FK_NOTIFICATION_PREFERENCE_CHANNEL",
                        column: x => x.ChannelCode,
                        principalTable: "NotificationChannels",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NOTIFICATION_PREFERENCE_TYPE",
                        column: x => x.TypeCode,
                        principalTable: "NotificationTypes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RecipientUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TypeCode = table.Column<string>(type: "varchar(10)", nullable: false),
                    SeverityCode = table.Column<string>(type: "varchar(10)", nullable: false),
                    Title = table.Column<string>(type: "varchar(120)", nullable: false),
                    Message = table.Column<string>(type: "varchar(512)", nullable: false),
                    Payload = table.Column<string>(type: "jsonb", nullable: true),
                    RelatedEntityType = table.Column<string>(type: "varchar(50)", nullable: true),
                    RelatedEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    HealthInstitutionId = table.Column<int>(type: "int", nullable: true),
                    SourceEventId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NOTIFICATION", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NOTIFICATION_SEVERITY",
                        column: x => x.SeverityCode,
                        principalTable: "NotificationSeverities",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NOTIFICATION_TYPE",
                        column: x => x.TypeCode,
                        principalTable: "NotificationTypes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NOTIFICATION_PREFERENCE_CHANNEL",
                table: "NotificationPreferences",
                column: "ChannelCode");

            migrationBuilder.CreateIndex(
                name: "IX_NOTIFICATION_PREFERENCE_TYPE",
                table: "NotificationPreferences",
                column: "TypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_NOTIFICATION_RECIPIENT_CREATED",
                table: "Notifications",
                columns: new[] { "RecipientUserId", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_NOTIFICATION_RECIPIENT_UNREAD",
                table: "Notifications",
                column: "RecipientUserId",
                filter: "\"ReadAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_NOTIFICATION_SEVERITY",
                table: "Notifications",
                column: "SeverityCode");

            migrationBuilder.CreateIndex(
                name: "IX_NOTIFICATION_TYPE",
                table: "Notifications",
                column: "TypeCode");

            migrationBuilder.CreateIndex(
                name: "UX_NOTIFICATION_SOURCE_EVENT",
                table: "Notifications",
                column: "SourceEventId",
                unique: true,
                filter: "\"SourceEventId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationPreferences");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "NotificationChannels");

            migrationBuilder.DropTable(
                name: "NotificationSeverities");

            migrationBuilder.DropTable(
                name: "NotificationTypes");
        }
    }
}
