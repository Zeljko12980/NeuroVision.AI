using BuildingBlocks.Persistence;
using PatientService.Application.Common.Interfaces;
using PatientService.Infrastructure.Services;

namespace PatientService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("patientdb")
            ?? throw new InvalidOperationException(
                "Connection string 'patientdb' not found.");

        services.AddDbContext<PatientDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddBuildingBlocksPersistence<PatientDbContext>(connectionString);

        services.AddScoped<IPatientWriteStore, PatientWriteStore>();
        services.AddScoped(typeof(IPatientReadStore<>), typeof(PatientReadStore<>));
        services.AddScoped(typeof(IRepository<,>), typeof(PatientRepository<,>));
        services.AddScoped<IFileStorageService, FileStorageService>();
        services.AddScoped<ISequenceStore, SequenceStore>();

        services.AddSingleton<IPatientSql<PatientResponse>, PatientSql>();
        services.AddSingleton<IPatientSql<PatientStatusResponse>, StatusSql>();
        services.AddSingleton<IPatientSql<GenderResponse>, GenderSql>();
        services.AddSingleton<IPatientSql<BloodTypeResponse>, BloodTypeSql>();
        services.AddSingleton<IPatientSql<LanguageResponse>, LanguageSql>();
        services.AddSingleton<IPatientSql<AllergyResponse>, AllergySql>();
        services.AddSingleton<IPatientSql<ConditionResponse>, ConditionSql>();
        services.AddSingleton<IPatientSql<InsurancePayerResponse>, InsurancePayerSql>();
        services.AddSingleton<IPatientSql<RelationshipTypeResponse>, RelationshipTypeSql>();
        services.AddSingleton<IPatientSql<ConsentTypeResponse>, ConsentTypeSql>();
        services.AddSingleton<IPatientSql<PatientStatusHistoryResponse>, PatientStatusHistorySql>();
        services.AddSingleton<IPatientSql<PatientAffiliationHistoryResponse>, PatientAffiliationHistorySql>();
        services.AddSingleton<IPatientSql<PatientInsuranceHistoryResponse>, PatientInsuranceHistorySql>();
        services.AddSingleton<IPatientSql<PatientDoctorAssignmentHistoryResponse>, PatientDoctorAssignmentHistorySql>();
        services.AddSingleton<IPatientSql<PatientLanguageCoverageResponse>, PatientLanguageCoverageSql>();
        services.AddSingleton<IPatientSql<PatientAllergyCoverageResponse>, PatientAllergyCoverageSql>();
        services.AddSingleton<IPatientSql<PatientConditionCoverageResponse>, PatientConditionCoverageSql>();
        services.AddSingleton<IPatientSql<PatientConsentCoverageResponse>, PatientConsentCoverageSql>();
        services.AddSingleton<IPatientSql<PatientEmergencyContactResponse>, PatientEmergencyContactSql>();

        return services;
    }
}
