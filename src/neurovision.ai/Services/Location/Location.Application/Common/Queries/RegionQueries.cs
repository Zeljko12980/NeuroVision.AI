
namespace LocationService.Application.Common.Queries
{
    public static class RegionQueries
    {
        public const string GetByKey = """
        SELECT *
        FROM "Regions"
        WHERE "TypeCode" = @TypeCode AND "Code" = @Code;
        """;

        public const string Exists = """
        SELECT COUNT(*)
        FROM "Regions"
        WHERE "TypeCode" = @TypeCode AND "Code" = @Code;
        """;

        public const string Count = """
        SELECT COUNT(*)
        FROM "Regions"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%');
        """;

        public const string GetPaged = """
        SELECT *
        FROM "Regions"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%')
        ORDER BY "Name"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
    }
}
