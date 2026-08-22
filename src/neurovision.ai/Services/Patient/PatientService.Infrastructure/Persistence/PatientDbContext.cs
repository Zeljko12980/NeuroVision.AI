namespace PatientService.Infrastructure.Persistence;

public class PatientDbContext : DbContext
{
    static PatientDbContext()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public PatientDbContext(DbContextOptions<PatientDbContext> options)
        : base(options)
    {
    }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<PatientStatus> PatientStatuses => Set<PatientStatus>();
    public DbSet<Gender> Genders => Set<Gender>();
    public DbSet<BloodType> BloodTypes => Set<BloodType>();
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<Allergy> Allergies => Set<Allergy>();
    public DbSet<Condition> Conditions => Set<Condition>();
    public DbSet<InsurancePayer> InsurancePayers => Set<InsurancePayer>();
    public DbSet<RelationshipType> RelationshipTypes => Set<RelationshipType>();
    public DbSet<ConsentType> ConsentTypes => Set<ConsentType>();
    public DbSet<PatientStatusHistory> PatientStatusHistories => Set<PatientStatusHistory>();
    public DbSet<PatientAffiliationHistory> PatientAffiliationHistories => Set<PatientAffiliationHistory>();
    public DbSet<PatientInsuranceHistory> PatientInsuranceHistories => Set<PatientInsuranceHistory>();
    public DbSet<PatientDoctorAssignmentHistory> PatientDoctorAssignmentHistories => Set<PatientDoctorAssignmentHistory>();
    public DbSet<PatientLanguageCoverage> PatientLanguageCoverages => Set<PatientLanguageCoverage>();
    public DbSet<PatientAllergyCoverage> PatientAllergyCoverages => Set<PatientAllergyCoverage>();
    public DbSet<PatientConditionCoverage> PatientConditionCoverages => Set<PatientConditionCoverage>();
    public DbSet<PatientConsentCoverage> PatientConsentCoverages => Set<PatientConsentCoverage>();
    public DbSet<PatientEmergencyContact> PatientEmergencyContacts => Set<PatientEmergencyContact>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PatientDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
