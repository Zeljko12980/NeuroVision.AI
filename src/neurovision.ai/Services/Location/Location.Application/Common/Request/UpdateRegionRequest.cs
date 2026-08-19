
namespace LocationService.Application.Common.Request
{
    public class UpdateRegionRequest
    {
        public string Name { get; set; } = null!;
        public string? BelongsToCountryCode { get; set; }
        public string? HeadquartersCountryCode { get; set; }
        public int? AdministrativeSeatSettlementCode { get; set; }
    }
}
