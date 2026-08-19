
namespace LocationService.Application.Common.Queries
{
    public static class LocalCommunityQueries
    {
        public const string GetByKey = """
        SELECT *
        FROM "LocalCommunities"
        WHERE "CountryCode" = @CountryCode AND "MunicipalityCode" = @MunicipalityCode AND "Identifier" = @Identifier;
        """;

        public const string Exists = """
        SELECT COUNT(*)
        FROM "LocalCommunities"
        WHERE "CountryCode" = @CountryCode AND "MunicipalityCode" = @MunicipalityCode AND "Identifier" = @Identifier;
        """;

        public const string Count = """
        SELECT COUNT(*)
        FROM "LocalCommunities"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%');
        """;

        public const string GetPaged = """
        SELECT *
        FROM "LocalCommunities"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%')
        ORDER BY "Name"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
    }
}
