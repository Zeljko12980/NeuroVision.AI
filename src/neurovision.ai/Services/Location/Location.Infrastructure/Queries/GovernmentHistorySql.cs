namespace LocationService.Infrastructure.Queries;

internal sealed class GovernmentHistorySql : ILocationSql<GovernmentHistoryResponse>
{
    public string GetByKey => """
        SELECT *
        FROM "GovernmentHistories"
        WHERE "CountryCode" = @CountryCode AND "SequenceNumber" = @SequenceNumber;
        """;

    public string Exists => """
        SELECT COUNT(*)
        FROM "GovernmentHistories"
        WHERE "CountryCode" = @CountryCode AND "SequenceNumber" = @SequenceNumber;
        """;

    public string Count => """
        SELECT COUNT(*)
        FROM "GovernmentHistories";
        """;

    public string GetPaged => """
        SELECT *
        FROM "GovernmentHistories"
        ORDER BY "CountryCode", "SequenceNumber"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
}
