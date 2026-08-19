namespace LocationService.Application.Common.Request
{
    public class UpdateCountryRequest
    {
        public string Name { get; set; } = null!;

        public DateTime FoundingDate { get; set; }

        public int? CapitalSettlementCode { get; set; }

        public string? GovernmentTypeCode { get; set; }

        public int? CallingCode { get; set; }

        public byte[]? Anthem { get; set; }

        public byte[]? CoatOfArms { get; set; }

        public byte[]? Flag { get; set; }
    }
}
