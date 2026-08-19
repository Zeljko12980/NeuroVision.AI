
namespace LocationService.Application.Common.Request
{
    public class CreateMunicipalityRequest
    {
        public string CountryCode { get; set; } = null!;
        public int Code { get; set; }
        public string Name { get; set; } = null!;
        public int? SeatSettlementCode { get; set; }
    }
}
