
namespace LocationService.Application.Common.Queries
{
    public static class GovernmentTypeQueries
    {
        public const string GetByKey = """
        SELECT *
        FROM "GovernmentTypes"
        WHERE "Code" = @Code;
        """;

        public const string Exists = """
        SELECT COUNT(*)
        FROM "GovernmentTypes"
        WHERE "Code" = @Code;
        """;

        public const string Count = """
        SELECT COUNT(*)
        FROM "GovernmentTypes"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%');
        """;

        public const string GetPaged = """
        SELECT *
        FROM "GovernmentTypes"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%')
        ORDER BY "Name"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
    }
}
