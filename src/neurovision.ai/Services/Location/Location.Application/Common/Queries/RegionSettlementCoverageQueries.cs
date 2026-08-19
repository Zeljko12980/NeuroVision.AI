
namespace LocationService.Application.Common.Queries
{
    public static class RegionSettlementCoverageQueries
    {
        public const string GetByKey = """
        SELECT *
        FROM "RegionSettlementCoverages"
        WHERE "RegionTypeCode" = @RegionTypeCode AND "RegionCode" = @RegionCode AND "CountryCode" = @CountryCode AND "SettlementCode" = @SettlementCode;
        """;

        public const string Exists = """
        SELECT COUNT(*)
        FROM "RegionSettlementCoverages"
        WHERE "RegionTypeCode" = @RegionTypeCode AND "RegionCode" = @RegionCode AND "CountryCode" = @CountryCode AND "SettlementCode" = @SettlementCode;
        """;

        public const string Count = """
        SELECT COUNT(*)
        FROM "RegionSettlementCoverages";
        """;

        public const string GetPaged = """
        SELECT *
        FROM "RegionSettlementCoverages"
        ORDER BY "RegionTypeCode", "RegionCode", "CountryCode", "SettlementCode"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
    }
}
