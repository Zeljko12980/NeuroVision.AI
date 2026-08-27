using BuildingBlocks.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TumorDetectionService.Application.Common.Interfaces;
using TumorDetectionService.Application.Common.Options;
using TumorDetectionService.Infrastructure.Persistence;
using TumorDetectionService.Infrastructure.Persistence.Repositories;
using TumorDetectionService.Infrastructure.Services;

namespace TumorDetectionService.Infrastructure;

public static class DependencyInjectionExtension
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("tumordetectiondb")
            ?? throw new InvalidOperationException("Connection string 'tumordetectiondb' not found.");

        services.AddDbContext<TumorDetectionDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddBuildingBlocksPersistence<TumorDetectionDbContext>(connectionString);

        services.AddScoped<IBrainScanRepository, BrainScanRepository>();
        services.AddScoped<ITumorAnalysisRepository, TumorAnalysisRepository>();
        services.AddScoped<IAiModelVersionRepository, AiModelVersionRepository>();
        services.AddScoped<IAiModelTypeRepository, AiModelTypeRepository>();
        services.AddScoped<IAnalysisCommentRepository, AnalysisCommentRepository>();
        services.AddScoped<IAnalysisErrorLogRepository, AnalysisErrorLogRepository>();
        services.AddScoped<IClinicalCatalogRepository, ClinicalCatalogRepository>();
        services.AddScoped<IAnalysisClinicalFollowUpRepository, AnalysisClinicalFollowUpRepository>();
        services.AddScoped(typeof(IRepository<,>), typeof(TumorDetectionRepository<,>));

        services.AddScoped<IScanStorageService, ScanStorageService>();
        services.AddScoped<IModelStorageService, ModelStorageService>();
        services.Configure<MlAnalysisOptions>(configuration.GetSection("MlAnalysis"));
        services.AddScoped<IMlAnalysisService, MlPythonAnalysisService>();
        services.AddSingleton<IAnalysisJobRunner, AnalysisJobRunner>();
        services.AddPdfReportServices(configuration);

        return services;
    }
}
