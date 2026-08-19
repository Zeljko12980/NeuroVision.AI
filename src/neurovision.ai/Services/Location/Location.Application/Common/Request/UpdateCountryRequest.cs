
using Microsoft.AspNetCore.Http;

namespace LocationService.Application.Common.Request
{
    public class UpdateCountryRequest
    {
        public string Name { get; set; } = null!;

        public DateTime FoundingDate { get; set; }

        public int? CapitalSettlementCode { get; set; }

        public string? GovernmentTypeCode { get; set; }

        public int? CallingCode { get; set; }

        public IFormFile? Anthem { get; set; }

        public IFormFile? CoatOfArms { get; set; }

        public IFormFile? Flag { get; set; }
    }
}
