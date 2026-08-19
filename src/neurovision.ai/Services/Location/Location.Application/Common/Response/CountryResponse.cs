
namespace LocationService.Application.Common.Response
{
    public class CountryResponse
    {
        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        public DateTime FoundingDate { get; set; }


        public int? CapitalSettlementCode { get; set; }

        public string? CapitalSettlementName { get; set; }


        public string? GovernmentTypeCode { get; set; }

        public string? GovernmentTypeName { get; set; }


        public int? CallingCode { get; set; }


        public byte[]? Anthem { get; set; }

        public byte[]? CoatOfArms { get; set; }

        public byte[]? Flag { get; set; }


        public int SettlementCount { get; set; }

        public int MunicipalityCount { get; set; }

        public int HealthInstitutionCount { get; set; }
    }
}
