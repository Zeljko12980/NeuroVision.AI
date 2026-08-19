
namespace LocationService.Domain.Entities
{
    public class HealthInstitutionType
    {
        public string Code { get; set; } = null!;   
        public string Name { get; set; } = null!;  
        public string? Description { get; set; }   

        public ICollection<HealthInstitution> HealthInstitutions { get; set; } = new List<HealthInstitution>();
    }
}
