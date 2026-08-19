namespace LocationService.Infrastructure.Queries;

internal sealed class MunicipalitySettlementCoverageSql : ILocationSql<MunicipalitySettlementCoverageResponse>
{
    public string GetByKey => """
        SELECT *
        FROM "MunicipalitySettlementCoverages"
        WHERE "CountryCode" = @CountryCode AND "MunicipalityCode" = @MunicipalityCode AND "SettlementCode" = @SettlementCode;
        """;

    public string Exists => """
        SELECT COUNT(*)
        FROM "MunicipalitySettlementCoverages"
        WHERE "CountryCode" = @CountryCode AND "MunicipalityCode" = @MunicipalityCode AND "SettlementCode" = @SettlementCode;
        """;

    public string Count => """
        SELECT COUNT(*)
        FROM "MunicipalitySettlementCoverages";
        """;

    public string GetPaged => """
        SELECT *
        FROM "MunicipalitySettlementCoverages"
        ORDER BY "CountryCode", "MunicipalityCode", "SettlementCode"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
}
