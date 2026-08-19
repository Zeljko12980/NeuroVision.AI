
namespace LocationService.Application.Common.Queries
{
    public static class MunicipalityQueries
    {
        public const string GetByKey = """
        SELECT *
        FROM "Municipalities"
        WHERE "CountryCode" = @CountryCode AND "Code" = @Code;
        """;

        public const string Exists = """
        SELECT COUNT(*)
        FROM "Municipalities"
        WHERE "CountryCode" = @CountryCode AND "Code" = @Code;
        """;

        public const string Count = """
        SELECT COUNT(*)
        FROM "Municipalities"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%');
        """;

        public const string GetPaged = """
        SELECT *
        FROM "Municipalities"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%')
        ORDER BY "Name"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
    }
}
