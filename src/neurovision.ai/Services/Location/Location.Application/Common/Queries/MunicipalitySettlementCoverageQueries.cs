
namespace LocationService.Application.Common.Queries
{
    public static class MunicipalitySettlementCoverageQueries
    {
        public const string GetByKey = """
        SELECT *
        FROM "MunicipalitySettlementCoverages"
        WHERE "CountryCode" = @CountryCode AND "MunicipalityCode" = @MunicipalityCode AND "SettlementCode" = @SettlementCode;
        """;

        public const string Exists = """
        SELECT COUNT(*)
        FROM "MunicipalitySettlementCoverages"
        WHERE "CountryCode" = @CountryCode AND "MunicipalityCode" = @MunicipalityCode AND "SettlementCode" = @SettlementCode;
        """;

        public const string Count = """
        SELECT COUNT(*)
        FROM "MunicipalitySettlementCoverages";
        """;

        public const string GetPaged = """
        SELECT *
        FROM "MunicipalitySettlementCoverages"
        ORDER BY "CountryCode", "MunicipalityCode", "SettlementCode"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
    }
}
