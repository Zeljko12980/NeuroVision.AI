namespace LocationService.Infrastructure.Queries;

internal sealed class CountryCompositionSql : ILocationSql<CountryCompositionResponse>
{
    public string GetByKey => """
        SELECT *
        FROM "CountryCompositions"
        WHERE "UnionCountryCode" = @UnionCountryCode AND "MemberCountryCode" = @MemberCountryCode AND "SequenceNumber" = @SequenceNumber;
        """;

    public string Exists => """
        SELECT COUNT(*)
        FROM "CountryCompositions"
        WHERE "UnionCountryCode" = @UnionCountryCode AND "MemberCountryCode" = @MemberCountryCode AND "SequenceNumber" = @SequenceNumber;
        """;

    public string Count => """
        SELECT COUNT(*)
        FROM "CountryCompositions";
        """;

    public string GetPaged => """
        SELECT *
        FROM "CountryCompositions"
        ORDER BY "UnionCountryCode", "MemberCountryCode", "SequenceNumber"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
}
