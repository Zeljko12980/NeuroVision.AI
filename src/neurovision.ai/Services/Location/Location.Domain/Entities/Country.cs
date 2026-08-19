namespace LocationService.Domain.Entities;

public class Country
{
    public string Code { get; set; } = null!;          
    public string Name { get; set; } = null!;           
    public DateTime FoundingDate { get; set; }          
    public int? CapitalSettlementCode { get; set; }      
    public string? GovernmentTypeCode { get; set; }     
    public int? CallingCode { get; set; }                
    public byte[]? Anthem { get; set; }                  
    public byte[]? CoatOfArms { get; set; }               
    public byte[]? Flag { get; set; }                   

   
    public GovernmentType? GovernmentType { get; set; }
    public Settlement? CapitalSettlement { get; set; }

    public ICollection<Settlement> Settlements { get; set; } = new List<Settlement>();
    public ICollection<Municipality> Municipalities { get; set; } = new List<Municipality>();
    public ICollection<Capital> CapitalHistory { get; set; } = new List<Capital>();
    public ICollection<GovernmentHistory> GovernmentHistory { get; set; } = new List<GovernmentHistory>();
    public ICollection<Region> HomeRegions { get; set; } = new List<Region>();

    public ICollection<LegalSuccessor> SuccessorOf { get; set; } = new List<LegalSuccessor>();   
    public ICollection<LegalSuccessor> PredecessorOf { get; set; } = new List<LegalSuccessor>();  

    public ICollection<CountryComposition> MemberOfUnions { get; set; } = new List<CountryComposition>();  
    public ICollection<CountryComposition> UnionMembers { get; set; } = new List<CountryComposition>();

    public ICollection<HealthInstitution> HealthInstitutions { get; set; } = new List<HealthInstitution>();
}
