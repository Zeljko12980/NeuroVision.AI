
namespace LocationService.Application.Common.Response
{
    public class RegionCompositionResponse
    {
        public string ParentRegionTypeCode { get; set; } = null!;

        public short ParentRegionCode { get; set; }

        public string MemberRegionTypeCode { get; set; } = null!;

        public short MemberRegionCode { get; set; }
    }
}
