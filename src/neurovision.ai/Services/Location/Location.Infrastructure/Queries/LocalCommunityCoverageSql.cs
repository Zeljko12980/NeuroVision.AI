namespace LocationService.Infrastructure.Queries;

internal sealed class LocalCommunityCoverageSql : ILocationSql<LocalCommunityCoverageResponse>
{
    public string GetByKey => """
        SELECT *
        FROM "LocalCommunityCoverages"
        WHERE "CountryCode" = @CountryCode AND "MunicipalityCode" = @MunicipalityCode AND "LocalCommunityIdentifier" = @LocalCommunityIdentifier AND "SettlementCode" = @SettlementCode;
        """;

    public string Exists => """
        SELECT COUNT(*)
        FROM "LocalCommunityCoverages"
        WHERE "CountryCode" = @CountryCode AND "MunicipalityCode" = @MunicipalityCode AND "LocalCommunityIdentifier" = @LocalCommunityIdentifier AND "SettlementCode" = @SettlementCode;
        """;

    public string Count => """
        SELECT COUNT(*)
        FROM "LocalCommunityCoverages";
        """;

    public string GetPaged => """
        SELECT *
        FROM "LocalCommunityCoverages"
        ORDER BY "CountryCode", "MunicipalityCode", "LocalCommunityIdentifier", "SettlementCode"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
}
