
namespace LocationService.Application.Common.Request
{
    public class CreateRegionRequest
    {
        public string TypeCode { get; set; } = null!;
        public short Code { get; set; }
        public string Name { get; set; } = null!;
        public string? BelongsToCountryCode { get; set; }
        public string? HeadquartersCountryCode { get; set; }
        public int? AdministrativeSeatSettlementCode { get; set; }
    }
}
