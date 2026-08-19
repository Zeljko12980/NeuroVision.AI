
namespace LocationService.Application.Common.Request
{
    public class CreateLocalCommunityCoverageRequest
    {
        public string CountryCode { get; set; } = null!;
        public int MunicipalityCode { get; set; }
        public int LocalCommunityIdentifier { get; set; }
        public int SettlementCode { get; set; }
    }
}
