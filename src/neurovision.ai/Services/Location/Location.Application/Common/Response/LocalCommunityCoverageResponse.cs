
namespace LocationService.Application.Common.Response
{
    public class LocalCommunityCoverageResponse
    {
        public string CountryCode { get; set; } = null!;

        public int MunicipalityCode { get; set; }

        public int LocalCommunityIdentifier { get; set; }

        public int SettlementCode { get; set; }
    }
}
