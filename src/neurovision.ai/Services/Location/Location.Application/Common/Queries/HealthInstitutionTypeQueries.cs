
namespace LocationService.Application.Common.Queries
{
    public static class HealthInstitutionTypeQueries
    {
        public const string GetByKey = """
        SELECT *
        FROM "HealthInstitutionTypes"
        WHERE "Code" = @Code;
        """;

        public const string Exists = """
        SELECT COUNT(*)
        FROM "HealthInstitutionTypes"
        WHERE "Code" = @Code;
        """;

        public const string Count = """
        SELECT COUNT(*)
        FROM "HealthInstitutionTypes"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%');
        """;

        public const string GetPaged = """
        SELECT *
        FROM "HealthInstitutionTypes"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%')
        ORDER BY "Name"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
    }
}
