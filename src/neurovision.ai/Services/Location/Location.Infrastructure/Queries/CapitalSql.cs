namespace LocationService.Infrastructure.Queries;

internal sealed class CapitalSql : ILocationSql<CapitalResponse>
{
    public string GetByKey => """
        SELECT *
        FROM "Capitals"
        WHERE "CountryCode" = @CountryCode AND "SettlementCode" = @SettlementCode AND "SequenceNumber" = @SequenceNumber;
        """;

    public string Exists => """
        SELECT COUNT(*)
        FROM "Capitals"
        WHERE "CountryCode" = @CountryCode AND "SettlementCode" = @SettlementCode AND "SequenceNumber" = @SequenceNumber;
        """;

    public string Count => """
        SELECT COUNT(*)
        FROM "Capitals";
        """;

    public string GetPaged => """
        SELECT *
        FROM "Capitals"
        ORDER BY "CountryCode", "SettlementCode", "SequenceNumber"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
}
