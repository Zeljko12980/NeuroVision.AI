
namespace LocationService.Application.Common.Request
{
    public class CreateMunicipalitySettlementCoverageRequest
    {
        public string CountryCode { get; set; } = null!;
        public int MunicipalityCode { get; set; }
        public int SettlementCode { get; set; }
    }
}
