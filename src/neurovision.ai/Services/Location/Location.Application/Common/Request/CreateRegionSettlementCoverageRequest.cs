
namespace LocationService.Application.Common.Request
{
    public class CreateRegionSettlementCoverageRequest
    {
        public string RegionTypeCode { get; set; } = null!;
        public short RegionCode { get; set; }
        public string CountryCode { get; set; } = null!;
        public int SettlementCode { get; set; }
    }
}
