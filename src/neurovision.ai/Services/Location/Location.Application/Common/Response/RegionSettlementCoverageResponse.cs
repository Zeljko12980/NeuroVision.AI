
namespace LocationService.Application.Common.Response
{
    public class RegionSettlementCoverageResponse
    {
        public string RegionTypeCode { get; set; } = null!;

        public short RegionCode { get; set; }

        public string CountryCode { get; set; } = null!;

        public int SettlementCode { get; set; }
    }
}
