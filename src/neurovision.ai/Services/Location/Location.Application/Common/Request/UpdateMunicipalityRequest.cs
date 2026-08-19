
namespace LocationService.Application.Common.Request
{
    public class UpdateMunicipalityRequest
    {
        public string Name { get; set; } = null!;
        public int? SeatSettlementCode { get; set; }
    }
}
