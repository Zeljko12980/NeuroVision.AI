
namespace LocationService.Application.Common.Queries
{
    public static class CapitalQueries
    {
        public const string GetByKey = """
        SELECT *
        FROM "Capitals"
        WHERE "CountryCode" = @CountryCode AND "SettlementCode" = @SettlementCode AND "SequenceNumber" = @SequenceNumber;
        """;

        public const string Exists = """
        SELECT COUNT(*)
        FROM "Capitals"
        WHERE "CountryCode" = @CountryCode AND "SettlementCode" = @SettlementCode AND "SequenceNumber" = @SequenceNumber;
        """;

        public const string Count = """
        SELECT COUNT(*)
        FROM "Capitals";
        """;

        public const string GetPaged = """
        SELECT *
        FROM "Capitals"
        ORDER BY "CountryCode", "SettlementCode", "SequenceNumber"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
    }
}
