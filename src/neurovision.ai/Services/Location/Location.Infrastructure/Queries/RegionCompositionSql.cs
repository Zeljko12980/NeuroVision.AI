namespace LocationService.Infrastructure.Queries;

internal sealed class RegionCompositionSql : ILocationSql<RegionCompositionResponse>
{
    public string GetByKey => """
        SELECT *
        FROM "RegionCompositions"
        WHERE "ParentRegionTypeCode" = @ParentRegionTypeCode AND "ParentRegionCode" = @ParentRegionCode AND "MemberRegionTypeCode" = @MemberRegionTypeCode AND "MemberRegionCode" = @MemberRegionCode;
        """;

    public string Exists => """
        SELECT COUNT(*)
        FROM "RegionCompositions"
        WHERE "ParentRegionTypeCode" = @ParentRegionTypeCode AND "ParentRegionCode" = @ParentRegionCode AND "MemberRegionTypeCode" = @MemberRegionTypeCode AND "MemberRegionCode" = @MemberRegionCode;
        """;

    public string Count => """
        SELECT COUNT(*)
        FROM "RegionCompositions";
        """;

    public string GetPaged => """
        SELECT *
        FROM "RegionCompositions"
        ORDER BY "ParentRegionTypeCode", "ParentRegionCode", "MemberRegionTypeCode", "MemberRegionCode"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
}
