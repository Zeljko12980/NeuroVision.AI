
namespace LocationService.Application.Common.Request
{
    public class UpdateHealthInstitutionRequest
    {
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
