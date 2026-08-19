
namespace LocationService.Application.Common.Queries
{
    public static class SettlementQueries
    {
        public const string GetByKey = """
        SELECT *
        FROM "Settlements"
        WHERE "CountryCode" = @CountryCode AND "Code" = @Code;
        """;

        public const string Exists = """
        SELECT COUNT(*)
        FROM "Settlements"
        WHERE "CountryCode" = @CountryCode AND "Code" = @Code;
        """;

        public const string Count = """
        SELECT COUNT(*)
        FROM "Settlements"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%');
        """;

        public const string GetPaged = """
        SELECT *
        FROM "Settlements"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%')
        ORDER BY "Name"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
    }
}
