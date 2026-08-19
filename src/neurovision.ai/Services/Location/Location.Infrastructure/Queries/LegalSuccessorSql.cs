namespace LocationService.Infrastructure.Queries;

internal sealed class LegalSuccessorSql : ILocationSql<LegalSuccessorResponse>
{
    public string GetByKey => """
        SELECT *
        FROM "LegalSuccessors"
        WHERE "SuccessorCountryCode" = @SuccessorCountryCode AND "PredecessorCountryCode" = @PredecessorCountryCode;
        """;

    public string Exists => """
        SELECT COUNT(*)
        FROM "LegalSuccessors"
        WHERE "SuccessorCountryCode" = @SuccessorCountryCode AND "PredecessorCountryCode" = @PredecessorCountryCode;
        """;

    public string Count => """
        SELECT COUNT(*)
        FROM "LegalSuccessors";
        """;

    public string GetPaged => """
        SELECT *
        FROM "LegalSuccessors"
        ORDER BY "SuccessorCountryCode", "PredecessorCountryCode"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
}
