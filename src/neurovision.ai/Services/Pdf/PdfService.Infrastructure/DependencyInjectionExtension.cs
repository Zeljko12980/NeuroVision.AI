using BuildingBlocks.Persistence;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PdfService.Application.Common.Interfaces;
using PdfService.Infrastructure.Data;
using PdfService.Infrastructure.Repositories;
using PdfService.Infrastructure.Services;

namespace PdfService.Infrastructure;

public static class DependencyInjectionExtension
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("pdfdb")
            ?? throw new InvalidOperationException(
                "Connection string 'pdfdb' not found.");

        services.AddDbContext<PdfDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped(typeof(IRepository<,>), typeof(PdfRepository<,>));
        services.AddBuildingBlocksPersistence<PdfDbContext>(connectionString);

        services.AddScoped<IPdfGenerator, HtmlPdfGenerator>();
        services.AddScoped<IPdfSigningService, PdfSigningService>();
        services.AddScoped<ICertificateFileParser, CertificateFileParser>();
        services.AddScoped<IPdfTemplateReadStore, PdfTemplateReadStore>();
        services.AddScoped<ICertificateReadStore, CertificateReadStore>();
        services.AddScoped<ICertificateStorage, CertificateStorage>();

        var dataProtectionKeysPath = Path.Combine(AppContext.BaseDirectory, "dp-keys");
        Directory.CreateDirectory(dataProtectionKeysPath);

        services.AddDataProtection()
            .SetApplicationName("PdfService")
            .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysPath));

        services.AddSingleton<ICertificatePasswordProtector, CertificatePasswordProtector>();

        return services;
    }
}
