
namespace LocationService.Application.Common.Response
{
    public class SettlementResponse
    {
        public string CountryCode { get; set; } = null!;

        public int Code { get; set; }

        public string Name { get; set; } = null!;

        public string? PostalCode { get; set; }
    }
}
