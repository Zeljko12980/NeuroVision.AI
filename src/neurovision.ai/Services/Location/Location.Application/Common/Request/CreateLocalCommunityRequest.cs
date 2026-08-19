
namespace LocationService.Application.Common.Request
{
    public class CreateLocalCommunityRequest
    {
        public string CountryCode { get; set; } = null!;
        public int MunicipalityCode { get; set; }
        public int Identifier { get; set; }
        public string Name { get; set; } = null!;
        public int? OfficeSettlementCode { get; set; }
    }
}
