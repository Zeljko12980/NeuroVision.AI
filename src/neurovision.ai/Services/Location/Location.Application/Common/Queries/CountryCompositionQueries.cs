
namespace LocationService.Application.Common.Queries
{
    public static class CountryCompositionQueries
    {
        public const string GetByKey = """
        SELECT *
        FROM "CountryCompositions"
        WHERE "UnionCountryCode" = @UnionCountryCode AND "MemberCountryCode" = @MemberCountryCode AND "SequenceNumber" = @SequenceNumber;
        """;

        public const string Exists = """
        SELECT COUNT(*)
        FROM "CountryCompositions"
        WHERE "UnionCountryCode" = @UnionCountryCode AND "MemberCountryCode" = @MemberCountryCode AND "SequenceNumber" = @SequenceNumber;
        """;

        public const string Count = """
        SELECT COUNT(*)
        FROM "CountryCompositions";
        """;

        public const string GetPaged = """
        SELECT *
        FROM "CountryCompositions"
        ORDER BY "UnionCountryCode", "MemberCountryCode", "SequenceNumber"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
    }
}
