namespace LocationService.Domain.Entities;

public class RegionComposition
{
    public string ParentRegionTypeCode { get; private set; } = null!;
    public short ParentRegionCode { get; private set; }
    public string MemberRegionTypeCode { get; private set; } = null!;
    public short MemberRegionCode { get; private set; }

    public Region ParentRegion { get; private set; } = null!;
    public Region MemberRegion { get; private set; } = null!;

    private RegionComposition()
    {
    }

    public static RegionComposition Create(
        string parentRegionTypeCode,
        short parentRegionCode,
        string memberRegionTypeCode,
        short memberRegionCode)
    {
        if (parentRegionCode <= 0)
            throw new ArgumentException("Parent region code must be greater than zero.", nameof(parentRegionCode));

        if (memberRegionCode <= 0)
            throw new ArgumentException("Member region code must be greater than zero.", nameof(memberRegionCode));

        return new RegionComposition
        {
            ParentRegionTypeCode = Guard.NotEmpty(parentRegionTypeCode, nameof(parentRegionTypeCode)),
            ParentRegionCode = parentRegionCode,
            MemberRegionTypeCode = Guard.NotEmpty(memberRegionTypeCode, nameof(memberRegionTypeCode)),
            MemberRegionCode = memberRegionCode
        };
    }
}
