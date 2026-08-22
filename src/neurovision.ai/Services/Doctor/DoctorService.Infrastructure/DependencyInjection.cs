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

        services.AddSingleton<IDoctorSql<DoctorResponse>, DoctorSql>();
        services.AddSingleton<IDoctorSql<SpecializationResponse>>(_ => new LookupSql<SpecializationResponse>("Specializations"));
        services.AddSingleton<IDoctorSql<LanguageResponse>>(_ => new LookupSql<LanguageResponse>("Languages"));
        services.AddSingleton<IDoctorSql<DegreeTypeResponse>>(_ => new LookupSql<DegreeTypeResponse>("DegreeTypes"));
        services.AddSingleton<IDoctorSql<LicenseAuthorityResponse>>(_ => new LookupSql<LicenseAuthorityResponse>("LicenseAuthorities"));

        return services;
    }
}
