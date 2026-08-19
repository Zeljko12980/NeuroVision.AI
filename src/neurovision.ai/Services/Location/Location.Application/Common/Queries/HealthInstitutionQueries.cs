
namespace LocationService.Application.Common.Queries
{
    public static class HealthInstitutionQueries
    {
        public const string GetByKey = """
        SELECT *
        FROM "HealthInstitutions"
        WHERE "Id" = @Id;
        """;

        public const string Exists = """
        SELECT COUNT(*)
        FROM "HealthInstitutions"
        WHERE "Id" = @Id;
        """;

        public const string Count = """
        SELECT COUNT(*)
        FROM "HealthInstitutions"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%');
        """;

        public const string GetPaged = """
        SELECT *
        FROM "HealthInstitutions"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%')
        ORDER BY "Name"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
    }
}
