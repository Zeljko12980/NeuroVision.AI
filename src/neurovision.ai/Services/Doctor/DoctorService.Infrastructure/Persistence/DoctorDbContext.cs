using DoctorService.Domain.Entities;

namespace DoctorService.Infrastructure.Persistence;

public class DoctorDbContext : DbContext
{
    static DoctorDbContext()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public DoctorDbContext(DbContextOptions<DoctorDbContext> options)
        : base(options)
    {
    }

    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<DoctorStatus> DoctorStatuses => Set<DoctorStatus>();
    public DbSet<Specialization> Specializations => Set<Specialization>();
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<DegreeType> DegreeTypes => Set<DegreeType>();
    public DbSet<LicenseAuthority> LicenseAuthorities => Set<LicenseAuthority>();
    public DbSet<DoctorStatusHistory> DoctorStatusHistories => Set<DoctorStatusHistory>();
    public DbSet<DoctorLicenseHistory> DoctorLicenseHistories => Set<DoctorLicenseHistory>();
    public DbSet<DoctorAffiliationHistory> DoctorAffiliationHistories => Set<DoctorAffiliationHistory>();
    public DbSet<DoctorLanguageCoverage> DoctorLanguageCoverages => Set<DoctorLanguageCoverage>();
    public DbSet<DoctorDegreeCoverage> DoctorDegreeCoverages => Set<DoctorDegreeCoverage>();
    public DbSet<DoctorSpecializationCoverage> DoctorSpecializationCoverages => Set<DoctorSpecializationCoverage>();
    public DbSet<WorkingSlot> WorkingSlots => Set<WorkingSlot>();
    public DbSet<DoctorReview> DoctorReviews => Set<DoctorReview>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DoctorDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
