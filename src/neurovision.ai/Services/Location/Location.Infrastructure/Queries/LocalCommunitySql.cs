namespace LocationService.Infrastructure.Queries;

internal sealed class LocalCommunitySql : ILocationSql<LocalCommunityResponse>
{
    public string GetByKey => """
        SELECT *
        FROM "LocalCommunities"
        WHERE "CountryCode" = @CountryCode AND "MunicipalityCode" = @MunicipalityCode AND "Identifier" = @Identifier;
        """;

    public string Exists => """
        SELECT COUNT(*)
        FROM "LocalCommunities"
        WHERE "CountryCode" = @CountryCode AND "MunicipalityCode" = @MunicipalityCode AND "Identifier" = @Identifier;
        """;

    public string Count => """
        SELECT COUNT(*)
        FROM "LocalCommunities"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%');
        """;

    public string GetPaged => """
        SELECT *
        FROM "LocalCommunities"
        WHERE (@Search IS NULL OR "Name" ILIKE '%' || @Search || '%')
        ORDER BY "Name"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
}
