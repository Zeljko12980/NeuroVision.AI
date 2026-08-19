
namespace LocationService.Application.Common.Queries
{
    public static class GovernmentHistoryQueries
    {
        public const string GetByKey = """
        SELECT *
        FROM "GovernmentHistories"
        WHERE "CountryCode" = @CountryCode AND "SequenceNumber" = @SequenceNumber;
        """;

        public const string Exists = """
        SELECT COUNT(*)
        FROM "GovernmentHistories"
        WHERE "CountryCode" = @CountryCode AND "SequenceNumber" = @SequenceNumber;
        """;

        public const string Count = """
        SELECT COUNT(*)
        FROM "GovernmentHistories";
        """;

        public const string GetPaged = """
        SELECT *
        FROM "GovernmentHistories"
        ORDER BY "CountryCode", "SequenceNumber"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
    }
}
