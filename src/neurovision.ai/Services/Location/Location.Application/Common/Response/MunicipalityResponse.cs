
namespace LocationService.Application.Common.Response
{
    public class MunicipalityResponse
    {
        public string CountryCode { get; set; } = null!;

        public int Code { get; set; }

        public string Name { get; set; } = null!;

        public int? SeatSettlementCode { get; set; }
    }
}
