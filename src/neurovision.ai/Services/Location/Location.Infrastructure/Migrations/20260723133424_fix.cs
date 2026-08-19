using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LocationService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GovernmentTypes",
                columns: table => new
                {
                    Code = table.Column<string>(type: "varchar(10)", nullable: false),
                    Name = table.Column<string>(type: "varchar(120)", nullable: false),
                    Description = table.Column<string>(type: "varchar(256)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GOVERNMENT_TYPE", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "HealthInstitutionTypes",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(1)", nullable: false),
                    Name = table.Column<string>(type: "varchar(120)", nullable: false),
                    Description = table.Column<string>(type: "varchar(256)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthInstitutionTypes", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "RegionTypes",
                columns: table => new
                {
                    Code = table.Column<string>(type: "varchar(10)", nullable: false),
                    Name = table.Column<string>(type: "varchar(120)", nullable: false),
                    Description = table.Column<string>(type: "varchar(265)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REGION_TYPE", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Capitals",
                columns: table => new
                {
                    CountryCode = table.Column<string>(type: "char(3)", nullable: false),
                    SettlementCode = table.Column<int>(type: "int", nullable: false),
                    SequenceNumber = table.Column<decimal>(type: "numeric(1,0)", nullable: false),
                    From = table.Column<DateTime>(type: "timestamp", nullable: false),
                    To = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CAPITAL", x => new { x.CountryCode, x.SettlementCode, x.SequenceNumber });
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", nullable: false),
                    Name = table.Column<string>(type: "varchar(120)", nullable: false),
                    FoundingDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    CapitalSettlementCode = table.Column<int>(type: "int", nullable: true),
                    GovernmentTypeCode = table.Column<string>(type: "char(1)", nullable: true),
                    CallingCode = table.Column<decimal>(type: "numeric(5,0)", nullable: true),
                    Anthem = table.Column<byte[]>(type: "bytea", nullable: true),
                    CoatOfArms = table.Column<byte[]>(type: "bytea", nullable: true),
                    Flag = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COUNTRY", x => x.Code);
                    table.ForeignKey(
                        name: "FK_COUNTRY_GOVERNMENT_TYPE",
                        column: x => x.GovernmentTypeCode,
                        principalTable: "GovernmentTypes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CountryCompositions",
                columns: table => new
                {
                    UnionCountryCode = table.Column<string>(type: "char(3)", nullable: false),
                    MemberCountryCode = table.Column<string>(type: "char(3)", nullable: false),
                    SequenceNumber = table.Column<decimal>(type: "numeric(2,0)", nullable: false),
                    From = table.Column<DateTime>(type: "timestamp", nullable: false),
                    To = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COUNTRY_COMPOSITION", x => new { x.MemberCountryCode, x.UnionCountryCode, x.SequenceNumber });
                    table.ForeignKey(
                        name: "FK_COUNTRY_COMPOSITION_MEMBER_COUNTRY",
                        column: x => x.MemberCountryCode,
                        principalTable: "Countries",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_COUNTRY_COMPOSITION_UNION_COUNTRY",
                        column: x => x.UnionCountryCode,
                        principalTable: "Countries",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GovernmentHistories",
                columns: table => new
                {
                    CountryCode = table.Column<string>(type: "char(3)", nullable: false),
                    SequenceNumber = table.Column<decimal>(type: "numeric(2,0)", nullable: false),
                    GovernmentTypeCode = table.Column<string>(type: "char(1)", nullable: false),
                    From = table.Column<DateTime>(type: "timestamp", nullable: false),
                    To = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GOVERNMENT_HISTORY", x => new { x.CountryCode, x.SequenceNumber });
                    table.ForeignKey(
                        name: "FK_GOVERNMENT_HISTORY_COUNTRY",
                        column: x => x.CountryCode,
                        principalTable: "Countries",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GOVERNMENT_HISTORY_GOVERNMENT_TYPE",
                        column: x => x.GovernmentTypeCode,
                        principalTable: "GovernmentTypes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LegalSuccessors",
                columns: table => new
                {
                    SuccessorCountryCode = table.Column<string>(type: "char(3)", nullable: false),
                    PredecessorCountryCode = table.Column<string>(type: "char(3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LEGAL_SUCCESSOR", x => new { x.PredecessorCountryCode, x.SuccessorCountryCode });
                    table.ForeignKey(
                        name: "FK_LEGAL_SUCCESSOR_PREDECESSOR_COUNTRY",
                        column: x => x.PredecessorCountryCode,
                        principalTable: "Countries",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LEGAL_SUCCESSOR_SUCCESSOR_COUNTRY",
                        column: x => x.SuccessorCountryCode,
                        principalTable: "Countries",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Settlements",
                columns: table => new
                {
                    CountryCode = table.Column<string>(type: "char(3)", nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(120)", nullable: false),
                    PostalCode = table.Column<string>(type: "varchar(12)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SETTLEMENT", x => new { x.CountryCode, x.Code });
                    table.ForeignKey(
                        name: "FK_SETTLEMENT_COUNTRY",
                        column: x => x.CountryCode,
                        principalTable: "Countries",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HealthInstitutions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar(150)", nullable: false),
                    TypeCode = table.Column<string>(type: "char(1)", nullable: false),
                    CountryCode = table.Column<string>(type: "char(3)", nullable: false),
                    SettlementCode = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "varchar(200)", nullable: true),
                    BedCount = table.Column<int>(type: "int", nullable: true),
                    FoundingDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    Phone = table.Column<string>(type: "varchar(30)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthInstitutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HealthInstitution_Country",
                        column: x => x.CountryCode,
                        principalTable: "Countries",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HealthInstitution_HealthInstitutionType",
                        column: x => x.TypeCode,
                        principalTable: "HealthInstitutionTypes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HealthInstitution_Settlement",
                        columns: x => new { x.CountryCode, x.SettlementCode },
                        principalTable: "Settlements",
                        principalColumns: new[] { "CountryCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Municipalities",
                columns: table => new
                {
                    CountryCode = table.Column<string>(type: "char(3)", nullable: false),
                    Code = table.Column<decimal>(type: "numeric(3,0)", nullable: false),
                    Name = table.Column<string>(type: "varchar(120)", nullable: false),
                    SeatSettlementCode = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MUNICIPALITY", x => new { x.CountryCode, x.Code });
                    table.ForeignKey(
                        name: "FK_MUNICIPALITY_COUNTRY",
                        column: x => x.CountryCode,
                        principalTable: "Countries",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MUNICIPALITY_SEAT_SETTLEMENT",
                        columns: x => new { x.CountryCode, x.SeatSettlementCode },
                        principalTable: "Settlements",
                        principalColumns: new[] { "CountryCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    TypeCode = table.Column<string>(type: "char(1)", nullable: false),
                    Code = table.Column<short>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "varchar(120)", nullable: false),
                    BelongsToCountryCode = table.Column<string>(type: "char(3)", nullable: true),
                    HeadquartersCountryCode = table.Column<string>(type: "char(3)", nullable: true),
                    AdministrativeSeatSettlementCode = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REGION", x => new { x.TypeCode, x.Code });
                    table.ForeignKey(
                        name: "FK_REGION_ADMINISTRATIVE_SEAT_SETTLEMENT",
                        columns: x => new { x.HeadquartersCountryCode, x.AdministrativeSeatSettlementCode },
                        principalTable: "Settlements",
                        principalColumns: new[] { "CountryCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_REGION_COUNTRY",
                        column: x => x.BelongsToCountryCode,
                        principalTable: "Countries",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_REGION_TYPE",
                        column: x => x.TypeCode,
                        principalTable: "RegionTypes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LocalCommunities",
                columns: table => new
                {
                    CountryCode = table.Column<string>(type: "char(3)", nullable: false),
                    MunicipalityCode = table.Column<decimal>(type: "numeric(3,0)", nullable: false),
                    Identifier = table.Column<decimal>(type: "numeric(2,0)", nullable: false),
                    Name = table.Column<string>(type: "varchar(120)", nullable: false),
                    OfficeSettlementCode = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOCAL_COMMUNITY", x => new { x.CountryCode, x.MunicipalityCode, x.Identifier });
                    table.ForeignKey(
                        name: "FK_LOCAL_COMMUNITY_MUNICIPALITY",
                        columns: x => new { x.CountryCode, x.MunicipalityCode },
                        principalTable: "Municipalities",
                        principalColumns: new[] { "CountryCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LOCAL_COMMUNITY_OFFICE_SETTLEMENT",
                        columns: x => new { x.CountryCode, x.OfficeSettlementCode },
                        principalTable: "Settlements",
                        principalColumns: new[] { "CountryCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MunicipalitySettlementCoverages",
                columns: table => new
                {
                    CountryCode = table.Column<string>(type: "char(3)", nullable: false),
                    MunicipalityCode = table.Column<decimal>(type: "numeric(3,0)", nullable: false),
                    SettlementCode = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MUNICIPALITY_SETTLEMENT_COVERAGE", x => new { x.MunicipalityCode, x.CountryCode, x.SettlementCode });
                    table.ForeignKey(
                        name: "FK_MUNICIPALITY_SETTLEMENT_COVERAGE_MUNICIPALITY",
                        columns: x => new { x.CountryCode, x.MunicipalityCode },
                        principalTable: "Municipalities",
                        principalColumns: new[] { "CountryCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MUNICIPALITY_SETTLEMENT_COVERAGE_SETTLEMENT",
                        columns: x => new { x.CountryCode, x.SettlementCode },
                        principalTable: "Settlements",
                        principalColumns: new[] { "CountryCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegionCompositions",
                columns: table => new
                {
                    ParentRegionTypeCode = table.Column<string>(type: "char(1)", nullable: false),
                    ParentRegionCode = table.Column<short>(type: "smallint", nullable: false),
                    MemberRegionTypeCode = table.Column<string>(type: "char(1)", nullable: false),
                    MemberRegionCode = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REGION_COMPOSITION", x => new { x.ParentRegionTypeCode, x.ParentRegionCode, x.MemberRegionTypeCode, x.MemberRegionCode });
                    table.ForeignKey(
                        name: "FK_REGION_COMPOSITION_MEMBER_REGION",
                        columns: x => new { x.MemberRegionTypeCode, x.MemberRegionCode },
                        principalTable: "Regions",
                        principalColumns: new[] { "TypeCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_REGION_COMPOSITION_PARENT_REGION",
                        columns: x => new { x.ParentRegionTypeCode, x.ParentRegionCode },
                        principalTable: "Regions",
                        principalColumns: new[] { "TypeCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegionSettlementCoverages",
                columns: table => new
                {
                    RegionTypeCode = table.Column<string>(type: "char(1)", nullable: false),
                    RegionCode = table.Column<short>(type: "smallint", nullable: false),
                    CountryCode = table.Column<string>(type: "char(3)", nullable: false),
                    SettlementCode = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REGION_SETTLEMENT_COVERAGE", x => new { x.CountryCode, x.SettlementCode, x.RegionTypeCode, x.RegionCode });
                    table.ForeignKey(
                        name: "FK_REGION_SETTLEMENT_COVERAGE_REGION",
                        columns: x => new { x.RegionTypeCode, x.RegionCode },
                        principalTable: "Regions",
                        principalColumns: new[] { "TypeCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_REGION_SETTLEMENT_COVERAGE_SETTLEMENT",
                        columns: x => new { x.CountryCode, x.SettlementCode },
                        principalTable: "Settlements",
                        principalColumns: new[] { "CountryCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LocalCommunityCoverages",
                columns: table => new
                {
                    CountryCode = table.Column<string>(type: "char(3)", nullable: false),
                    MunicipalityCode = table.Column<decimal>(type: "numeric(3,0)", nullable: false),
                    LocalCommunityIdentifier = table.Column<decimal>(type: "numeric(2,0)", nullable: false),
                    SettlementCode = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOCAL_COMMUNITY_COVERAGE", x => new { x.MunicipalityCode, x.LocalCommunityIdentifier, x.CountryCode, x.SettlementCode });
                    table.ForeignKey(
                        name: "FK_LOCAL_COMMUNITY_COVERAGE_LOCAL_COMMUNITY",
                        columns: x => new { x.CountryCode, x.MunicipalityCode, x.LocalCommunityIdentifier },
                        principalTable: "LocalCommunities",
                        principalColumns: new[] { "CountryCode", "MunicipalityCode", "Identifier" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LOCAL_COMMUNITY_COVERAGE_SETTLEMENT",
                        columns: x => new { x.CountryCode, x.SettlementCode },
                        principalTable: "Settlements",
                        principalColumns: new[] { "CountryCode", "Code" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CAPITAL_COUNTRY",
                table: "Capitals",
                column: "CountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_CAPITAL_SETTLEMENT",
                table: "Capitals",
                columns: new[] { "CountryCode", "SettlementCode" });

            migrationBuilder.CreateIndex(
                name: "IX_COUNTRY_CAPITAL_SETTLEMENT",
                table: "Countries",
                columns: new[] { "Code", "CapitalSettlementCode" });

            migrationBuilder.CreateIndex(
                name: "IX_COUNTRY_GOVERNMENT_TYPE",
                table: "Countries",
                column: "GovernmentTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_COUNTRY_COMPOSITION_MEMBER_COUNTRY",
                table: "CountryCompositions",
                column: "MemberCountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_COUNTRY_COMPOSITION_UNION_COUNTRY",
                table: "CountryCompositions",
                column: "UnionCountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_GOVERNMENT_HISTORY_COUNTRY",
                table: "GovernmentHistories",
                column: "CountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_GOVERNMENT_HISTORY_GOVERNMENT_TYPE",
                table: "GovernmentHistories",
                column: "GovernmentTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_HealthInstitution_CountryCode",
                table: "HealthInstitutions",
                column: "CountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_HealthInstitution_Settlement",
                table: "HealthInstitutions",
                columns: new[] { "CountryCode", "SettlementCode" });

            migrationBuilder.CreateIndex(
                name: "IX_HealthInstitution_TypeCode",
                table: "HealthInstitutions",
                column: "TypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_LEGAL_SUCCESSOR_PREDECESSOR_COUNTRY",
                table: "LegalSuccessors",
                column: "PredecessorCountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_LEGAL_SUCCESSOR_SUCCESSOR_COUNTRY",
                table: "LegalSuccessors",
                column: "SuccessorCountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_LOCAL_COMMUNITY_MUNICIPALITY",
                table: "LocalCommunities",
                columns: new[] { "CountryCode", "MunicipalityCode" });

            migrationBuilder.CreateIndex(
                name: "IX_LOCAL_COMMUNITY_OFFICE_SETTLEMENT",
                table: "LocalCommunities",
                columns: new[] { "CountryCode", "OfficeSettlementCode" });

            migrationBuilder.CreateIndex(
                name: "IX_LOCAL_COMMUNITY_COVERAGE_LOCAL_COMMUNITY",
                table: "LocalCommunityCoverages",
                columns: new[] { "CountryCode", "MunicipalityCode", "LocalCommunityIdentifier" });

            migrationBuilder.CreateIndex(
                name: "IX_LOCAL_COMMUNITY_COVERAGE_SETTLEMENT",
                table: "LocalCommunityCoverages",
                columns: new[] { "CountryCode", "SettlementCode" });

            migrationBuilder.CreateIndex(
                name: "IX_MUNICIPALITY_COUNTRY",
                table: "Municipalities",
                column: "CountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_MUNICIPALITY_SEAT_SETTLEMENT",
                table: "Municipalities",
                columns: new[] { "CountryCode", "SeatSettlementCode" });

            migrationBuilder.CreateIndex(
                name: "IX_MUNICIPALITY_SETTLEMENT_COVERAGE_MUNICIPALITY",
                table: "MunicipalitySettlementCoverages",
                columns: new[] { "CountryCode", "MunicipalityCode" });

            migrationBuilder.CreateIndex(
                name: "IX_MUNICIPALITY_SETTLEMENT_COVERAGE_SETTLEMENT",
                table: "MunicipalitySettlementCoverages",
                columns: new[] { "CountryCode", "SettlementCode" });

            migrationBuilder.CreateIndex(
                name: "IX_REGION_COMPOSITION_MEMBER_REGION",
                table: "RegionCompositions",
                columns: new[] { "MemberRegionTypeCode", "MemberRegionCode" });

            migrationBuilder.CreateIndex(
                name: "IX_REGION_COMPOSITION_PARENT_REGION",
                table: "RegionCompositions",
                columns: new[] { "ParentRegionTypeCode", "ParentRegionCode" });

            migrationBuilder.CreateIndex(
                name: "IX_REGION_ADMINISTRATIVE_SEAT_SETTLEMENT",
                table: "Regions",
                columns: new[] { "HeadquartersCountryCode", "AdministrativeSeatSettlementCode" });

            migrationBuilder.CreateIndex(
                name: "IX_REGION_COUNTRY",
                table: "Regions",
                column: "BelongsToCountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_REGION_TYPE",
                table: "Regions",
                column: "TypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_REGION_SETTLEMENT_COVERAGE_REGION",
                table: "RegionSettlementCoverages",
                columns: new[] { "RegionTypeCode", "RegionCode" });

            migrationBuilder.CreateIndex(
                name: "IX_REGION_SETTLEMENT_COVERAGE_SETTLEMENT",
                table: "RegionSettlementCoverages",
                columns: new[] { "CountryCode", "SettlementCode" });

            migrationBuilder.CreateIndex(
                name: "IX_SETTLEMENT_COUNTRY",
                table: "Settlements",
                column: "CountryCode");

            migrationBuilder.AddForeignKey(
                name: "FK_CAPITAL_COUNTRY",
                table: "Capitals",
                column: "CountryCode",
                principalTable: "Countries",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CAPITAL_SETTLEMENT",
                table: "Capitals",
                columns: new[] { "CountryCode", "SettlementCode" },
                principalTable: "Settlements",
                principalColumns: new[] { "CountryCode", "Code" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_COUNTRY_CAPITAL_SETTLEMENT",
                table: "Countries",
                columns: new[] { "Code", "CapitalSettlementCode" },
                principalTable: "Settlements",
                principalColumns: new[] { "CountryCode", "Code" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SETTLEMENT_COUNTRY",
                table: "Settlements");

            migrationBuilder.DropTable(
                name: "Capitals");

            migrationBuilder.DropTable(
                name: "CountryCompositions");

            migrationBuilder.DropTable(
                name: "GovernmentHistories");

            migrationBuilder.DropTable(
                name: "HealthInstitutions");

            migrationBuilder.DropTable(
                name: "LegalSuccessors");

            migrationBuilder.DropTable(
                name: "LocalCommunityCoverages");

            migrationBuilder.DropTable(
                name: "MunicipalitySettlementCoverages");

            migrationBuilder.DropTable(
                name: "RegionCompositions");

            migrationBuilder.DropTable(
                name: "RegionSettlementCoverages");

            migrationBuilder.DropTable(
                name: "HealthInstitutionTypes");

            migrationBuilder.DropTable(
                name: "LocalCommunities");

            migrationBuilder.DropTable(
                name: "Regions");

            migrationBuilder.DropTable(
                name: "Municipalities");

            migrationBuilder.DropTable(
                name: "RegionTypes");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Settlements");

            migrationBuilder.DropTable(
                name: "GovernmentTypes");
        }
    }
}
