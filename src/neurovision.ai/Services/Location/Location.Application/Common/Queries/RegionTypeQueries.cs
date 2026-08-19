
namespace LocationService.Application.Common.Queries
{
    public static class RegionTypeQueries
    {
        public const string GetByKey = """
        SELECT *
        FROM "RegionTypes"
        WHERE "Code" = @Code;
        """;

        public const string Exists = """
        SELECT COUNT(*)
        FROM "RegionTypes"
        WHERE "Code" = @Code;
        """;

        public const string Count = """
        SELECT COUNT(*)
        FROM "RegionTypes"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%');
        """;

        public const string GetPaged = """
        SELECT *
        FROM "RegionTypes"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%')
        ORDER BY "Name"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
    }
}
