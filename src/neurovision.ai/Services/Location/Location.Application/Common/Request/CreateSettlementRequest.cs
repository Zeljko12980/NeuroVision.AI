
namespace LocationService.Application.Common.Request
{
    public class CreateSettlementRequest
    {
        public string CountryCode { get; set; } = null!;
        public int Code { get; set; }
        public string Name { get; set; } = null!;
        public string? PostalCode { get; set; }
    }
}
