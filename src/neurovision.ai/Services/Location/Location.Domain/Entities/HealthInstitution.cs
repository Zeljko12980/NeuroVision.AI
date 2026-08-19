namespace LocationService.Domain.Entities
{
    public class HealthInstitution
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

        public HealthInstitutionType Type { get; set; } = null!;
        public Country Country { get; set; } = null!;
        public Settlement Settlement { get; set; } = null!;
    }
}
