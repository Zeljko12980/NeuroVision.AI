
namespace LocationService.Application.Common.Queries
{
    public static class LegalSuccessorQueries
    {
        public const string GetByKey = """
        SELECT *
        FROM "LegalSuccessors"
        WHERE "SuccessorCountryCode" = @SuccessorCountryCode AND "PredecessorCountryCode" = @PredecessorCountryCode;
        """;

        public const string Exists = """
        SELECT COUNT(*)
        FROM "LegalSuccessors"
        WHERE "SuccessorCountryCode" = @SuccessorCountryCode AND "PredecessorCountryCode" = @PredecessorCountryCode;
        """;

        public const string Count = """
        SELECT COUNT(*)
        FROM "LegalSuccessors";
        """;

        public const string GetPaged = """
        SELECT *
        FROM "LegalSuccessors"
        ORDER BY "SuccessorCountryCode", "PredecessorCountryCode"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
    }
}
