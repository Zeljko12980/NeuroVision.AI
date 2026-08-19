
namespace LocationService.Application.Common.Queries
{
    public static class CountryQueries
    {
        public const string GetByCode = """
        SELECT *
        FROM "Countries"
        WHERE "Code" = @Code;
        """;

        public const string Exists = """
        SELECT COUNT(*)
        FROM "Countries"
        WHERE "Code" = @Code;
        """;

        public const string Count = """
        SELECT COUNT(*)
        FROM "Countries";
        """;


        public const string GetPaged = """
        SELECT *
        FROM "Countries"
        ORDER BY "Name"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
    }
}
