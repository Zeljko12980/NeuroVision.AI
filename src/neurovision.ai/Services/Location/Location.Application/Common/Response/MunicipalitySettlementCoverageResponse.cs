
namespace LocationService.Application.Common.Response
{
    public class MunicipalitySettlementCoverageResponse
    {
        public string CountryCode { get; set; } = null!;

        public int MunicipalityCode { get; set; }

        public int SettlementCode { get; set; }
    }
}
