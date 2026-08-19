
namespace LocationService.Application.Common.Request
{
    public class CreateRegionCompositionRequest
    {
        public string ParentRegionTypeCode { get; set; } = null!;
        public short ParentRegionCode { get; set; }
        public string MemberRegionTypeCode { get; set; } = null!;
        public short MemberRegionCode { get; set; }
    }
}
