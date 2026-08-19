using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LocationService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fix2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CountryCode",
                table: "Settlements",
                type: "varchar(3)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(3)");

            migrationBuilder.AlterColumn<string>(
                name: "RegionTypeCode",
                table: "RegionSettlementCoverages",
                type: "varchar(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(1)");

            migrationBuilder.AlterColumn<string>(
                name: "CountryCode",
                table: "RegionSettlementCoverages",
                type: "varchar(3)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(3)");

            migrationBuilder.AlterColumn<string>(
                name: "HeadquartersCountryCode",
                table: "Regions",
                type: "varchar(3)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(3)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BelongsToCountryCode",
                table: "Regions",
                type: "varchar(3)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(3)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TypeCode",
                table: "Regions",
                type: "varchar(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(1)");

            migrationBuilder.AlterColumn<string>(
                name: "MemberRegionTypeCode",
                table: "RegionCompositions",
                type: "varchar(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(1)");

            migrationBuilder.AlterColumn<string>(
                name: "ParentRegionTypeCode",
                table: "RegionCompositions",
                type: "varchar(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(1)");

            migrationBuilder.AlterColumn<string>(
                name: "CountryCode",
                table: "MunicipalitySettlementCoverages",
                type: "varchar(3)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(3)");

            migrationBuilder.AlterColumn<string>(
                name: "CountryCode",
                table: "Municipalities",
                type: "varchar(3)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(3)");

            migrationBuilder.AlterColumn<string>(
                name: "CountryCode",
                table: "LocalCommunityCoverages",
                type: "varchar(3)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(3)");

            migrationBuilder.AlterColumn<string>(
                name: "CountryCode",
                table: "LocalCommunities",
                type: "varchar(3)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(3)");

            migrationBuilder.AlterColumn<string>(
                name: "SuccessorCountryCode",
                table: "LegalSuccessors",
                type: "varchar(3)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(3)");

            migrationBuilder.AlterColumn<string>(
                name: "PredecessorCountryCode",
                table: "LegalSuccessors",
                type: "varchar(3)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(3)");

            migrationBuilder.AlterColumn<string>(
                name: "GovernmentTypeCode",
                table: "GovernmentHistories",
                type: "varchar(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(1)");

            migrationBuilder.AlterColumn<string>(
                name: "CountryCode",
                table: "GovernmentHistories",
                type: "varchar(3)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(3)");

            migrationBuilder.AlterColumn<string>(
                name: "UnionCountryCode",
                table: "CountryCompositions",
                type: "varchar(3)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(3)");

            migrationBuilder.AlterColumn<string>(
                name: "MemberCountryCode",
                table: "CountryCompositions",
                type: "varchar(3)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(3)");

            migrationBuilder.AlterColumn<string>(
                name: "GovernmentTypeCode",
                table: "Countries",
                type: "varchar(10)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(1)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Countries",
                type: "varchar(3)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(3)");

            migrationBuilder.AlterColumn<string>(
                name: "CountryCode",
                table: "Capitals",
                type: "varchar(3)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(3)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CountryCode",
                table: "Settlements",
                type: "char(3)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(3)");

            migrationBuilder.AlterColumn<string>(
                name: "RegionTypeCode",
                table: "RegionSettlementCoverages",
                type: "char(1)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(10)");

            migrationBuilder.AlterColumn<string>(
                name: "CountryCode",
                table: "RegionSettlementCoverages",
                type: "char(3)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(3)");

            migrationBuilder.AlterColumn<string>(
                name: "HeadquartersCountryCode",
                table: "Regions",
                type: "char(3)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(3)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BelongsToCountryCode",
                table: "Regions",
                type: "char(3)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(3)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TypeCode",
                table: "Regions",
                type: "char(1)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(10)");

            migrationBuilder.AlterColumn<string>(
                name: "MemberRegionTypeCode",
                table: "RegionCompositions",
                type: "char(1)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(10)");

            migrationBuilder.AlterColumn<string>(
                name: "ParentRegionTypeCode",
                table: "RegionCompositions",
                type: "char(1)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(10)");

            migrationBuilder.AlterColumn<string>(
                name: "CountryCode",
                table: "MunicipalitySettlementCoverages",
                type: "char(3)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(3)");

            migrationBuilder.AlterColumn<string>(
                name: "CountryCode",
                table: "Municipalities",
                type: "char(3)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(3)");

            migrationBuilder.AlterColumn<string>(
                name: "CountryCode",
                table: "LocalCommunityCoverages",
                type: "char(3)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(3)");

            migrationBuilder.AlterColumn<string>(
                name: "CountryCode",
                table: "LocalCommunities",
                type: "char(3)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(3)");

            migrationBuilder.AlterColumn<string>(
                name: "SuccessorCountryCode",
                table: "LegalSuccessors",
                type: "char(3)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(3)");

            migrationBuilder.AlterColumn<string>(
                name: "PredecessorCountryCode",
                table: "LegalSuccessors",
                type: "char(3)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(3)");

            migrationBuilder.AlterColumn<string>(
                name: "GovernmentTypeCode",
                table: "GovernmentHistories",
                type: "char(1)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(10)");

            migrationBuilder.AlterColumn<string>(
                name: "CountryCode",
                table: "GovernmentHistories",
                type: "char(3)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(3)");

            migrationBuilder.AlterColumn<string>(
                name: "UnionCountryCode",
                table: "CountryCompositions",
                type: "char(3)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(3)");

            migrationBuilder.AlterColumn<string>(
                name: "MemberCountryCode",
                table: "CountryCompositions",
                type: "char(3)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(3)");

            migrationBuilder.AlterColumn<string>(
                name: "GovernmentTypeCode",
                table: "Countries",
                type: "char(1)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Countries",
                type: "char(3)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(3)");

            migrationBuilder.AlterColumn<string>(
                name: "CountryCode",
                table: "Capitals",
                type: "char(3)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(3)");
        }
    }
}
