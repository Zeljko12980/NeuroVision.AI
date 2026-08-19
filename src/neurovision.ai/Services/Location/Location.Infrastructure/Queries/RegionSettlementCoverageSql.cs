namespace LocationService.Infrastructure.Queries;

internal sealed class RegionSettlementCoverageSql : ILocationSql<RegionSettlementCoverageResponse>
{
    public string GetByKey => """
        SELECT *
        FROM "RegionSettlementCoverages"
        WHERE "RegionTypeCode" = @RegionTypeCode AND "RegionCode" = @RegionCode AND "CountryCode" = @CountryCode AND "SettlementCode" = @SettlementCode;
        """;

    public string Exists => """
        SELECT COUNT(*)
        FROM "RegionSettlementCoverages"
        WHERE "RegionTypeCode" = @RegionTypeCode AND "RegionCode" = @RegionCode AND "CountryCode" = @CountryCode AND "SettlementCode" = @SettlementCode;
        """;

    public string Count => """
        SELECT COUNT(*)
        FROM "RegionSettlementCoverages";
        """;

    public string GetPaged => """
        SELECT *
        FROM "RegionSettlementCoverages"
        ORDER BY "RegionTypeCode", "RegionCode", "CountryCode", "SettlementCode"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
}
