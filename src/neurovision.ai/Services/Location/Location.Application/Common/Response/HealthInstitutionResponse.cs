
namespace LocationService.Application.Common.Response
{
    public class HealthInstitutionResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string TypeCode { get; set; } = null!;

        public string CountryCode { get; set; } = null!;

        public int SettlementCode { get; set; }

        public string? Address { get; set; }

        public int? BedCount { get; set; }

        public DateTime? FoundingDate { get; set; }

        public string? Phone { get; set; }
    }
}
