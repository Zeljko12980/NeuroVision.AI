
namespace LocationService.Application.Common.Response
{
    public class LocalCommunityResponse
    {
        public string CountryCode { get; set; } = null!;

        public int MunicipalityCode { get; set; }

        public int Identifier { get; set; }

        public string Name { get; set; } = null!;

        public int? OfficeSettlementCode { get; set; }
    }
}
