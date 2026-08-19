
namespace LocationService.Application.Common.Queries
{
    public static class RegionCompositionQueries
    {
        public const string GetByKey = """
        SELECT *
        FROM "RegionCompositions"
        WHERE "ParentRegionTypeCode" = @ParentRegionTypeCode AND "ParentRegionCode" = @ParentRegionCode AND "MemberRegionTypeCode" = @MemberRegionTypeCode AND "MemberRegionCode" = @MemberRegionCode;
        """;

        public const string Exists = """
        SELECT COUNT(*)
        FROM "RegionCompositions"
        WHERE "ParentRegionTypeCode" = @ParentRegionTypeCode AND "ParentRegionCode" = @ParentRegionCode AND "MemberRegionTypeCode" = @MemberRegionTypeCode AND "MemberRegionCode" = @MemberRegionCode;
        """;

        public const string Count = """
        SELECT COUNT(*)
        FROM "RegionCompositions";
        """;

        public const string GetPaged = """
        SELECT *
        FROM "RegionCompositions"
        ORDER BY "ParentRegionTypeCode", "ParentRegionCode", "MemberRegionTypeCode", "MemberRegionCode"
        LIMIT @PageSize
        OFFSET @Offset;
        """;
    }
}
