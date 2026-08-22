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

        services.AddSingleton<IPatientSql<PatientResponse>, PatientSql>();
        services.AddSingleton<IPatientSql<PatientStatusResponse>>(_ => new LookupSql<PatientStatusResponse>("PatientStatuses"));
        services.AddSingleton<IPatientSql<GenderResponse>>(_ => new LookupSql<GenderResponse>("Genders"));
        services.AddSingleton<IPatientSql<BloodTypeResponse>>(_ => new LookupSql<BloodTypeResponse>("BloodTypes"));
        services.AddSingleton<IPatientSql<LanguageResponse>>(_ => new LookupSql<LanguageResponse>("Languages"));
        services.AddSingleton<IPatientSql<AllergyResponse>>(_ => new LookupSql<AllergyResponse>("Allergies"));
        services.AddSingleton<IPatientSql<ConditionResponse>>(_ => new LookupSql<ConditionResponse>("Conditions"));
        services.AddSingleton<IPatientSql<InsurancePayerResponse>>(_ => new LookupSql<InsurancePayerResponse>("InsurancePayers"));
        services.AddSingleton<IPatientSql<RelationshipTypeResponse>>(_ => new LookupSql<RelationshipTypeResponse>("RelationshipTypes"));
        services.AddSingleton<IPatientSql<ConsentTypeResponse>>(_ => new LookupSql<ConsentTypeResponse>("ConsentTypes"));

        return services;
    }
}
