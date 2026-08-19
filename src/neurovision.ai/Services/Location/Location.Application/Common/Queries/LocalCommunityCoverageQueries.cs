
namespace LocationService.Application.Common.Queries
{
    public static class LocalCommunityCoverageQueries
    {
        public const string GetByKey = """
        SELECT *
        FROM "LocalCommunityCoverages"
        WHERE "CountryCode" = @CountryCode AND "MunicipalityCode" = @MunicipalityCode AND "LocalCommunityIdentifier" = @LocalCommunityIdentifier AND "SettlementCode" = @SettlementCode;
        """;

        public const string Exists = """
        SELECT COUNT(*)
        FROM "LocalCommunityCoverages"
        WHERE "CountryCode" = @CountryCode AND "MunicipalityCode" = @MunicipalityCode AND "LocalCommunityIdentifier" = @LocalCommunityIdentifier AND "SettlementCode" = @SettlementCode;
        """;

        public const string Count = """
        SELECT COUNT(*)
        FROM "LocalCommunityCoverages";
        """;

        public const string GetPaged = """
        SELECT *
        FROM "LocalCommunityCoverages"
        ORDER BY "CountryCode", "MunicipalityCode", "LocalCommunityIdentifier", "SettlementCode"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
    }
}
