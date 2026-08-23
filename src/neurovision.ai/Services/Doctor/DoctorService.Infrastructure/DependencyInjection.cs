using BuildingBlocks.Persistence;
using DoctorService.Application.Common.Interfaces;
using DoctorService.Application.Common.Response;
using DoctorService.Infrastructure.Persistence;
using DoctorService.Infrastructure.Queries;
using DoctorService.Infrastructure.Services;

namespace DoctorService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("doctordb")
            ?? throw new InvalidOperationException(
                "Connection string 'doctordb' not found.");

        services.AddDbContext<DoctorDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddBuildingBlocksPersistence<DoctorDbContext>(connectionString);

        services.AddScoped<IDoctorWriteStore, DoctorWriteStore>();
        services.AddScoped(typeof(IDoctorReadStore<>), typeof(DoctorReadStore<>));
        services.AddScoped(typeof(IRepository<,>), typeof(DoctorRepository<,>));
        services.AddScoped<IFileStorageService, FileStorageService>();
        services.AddScoped<ISequenceStore, SequenceStore>();

        services.AddSingleton<IDoctorSql<DoctorResponse>, DoctorSql>();
        services.AddSingleton<IDoctorSql<DoctorStatusResponse>, DoctorStatusSql>();
        services.AddSingleton<IDoctorSql<SpecializationResponse>, SpecializationSql>();
        services.AddSingleton<IDoctorSql<LanguageResponse>, DoctorLanguageSql>();
        services.AddSingleton<IDoctorSql<DegreeTypeResponse>, DegreeTypeSql>();
        services.AddSingleton<IDoctorSql<LicenseAuthorityResponse>, LicenseAuthoritySql>();
        services.AddSingleton<IDoctorSql<DoctorStatusHistoryResponse>, DoctorStatusHistorySql>();
        services.AddSingleton<IDoctorSql<DoctorLicenseHistoryResponse>, DoctorLicenseHistorySql>();
        services.AddSingleton<IDoctorSql<DoctorAffiliationHistoryResponse>, DoctorAffiliationHistorySql>();
        services.AddSingleton<IDoctorSql<DoctorLanguageCoverageResponse>, DoctorLanguageCoverageSql>();
        services.AddSingleton<IDoctorSql<DoctorDegreeCoverageResponse>, DoctorDegreeCoverageSql>();
        services.AddSingleton<IDoctorSql<DoctorSpecializationCoverageResponse>, DoctorSpecializationCoverageSql>();
        services.AddSingleton<IDoctorSql<WorkingSlotResponse>, WorkingSlotSql>();
        services.AddSingleton<IDoctorSql<DoctorReviewResponse>, DoctorReviewSql>();

        return services;
    }
}
