using PdfService.Grpc;

namespace MailService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<SmtpSettings>(configuration.GetSection(SmtpSettings.SectionName));

        var pdfUrl = configuration["PdfService:GrpcUrl"]
            ?? throw new InvalidOperationException("PdfService:GrpcUrl is not configured.");

        services.AddGrpcClient<PdfGenerator.PdfGeneratorClient>(options =>
        {
            options.Address = new Uri(pdfUrl);
        });

        services.AddScoped<IEmailSender, SmtpEmailSender>();
        services.AddScoped<IDocumentGenerator, PdfDocumentGenerator>();

        return services;
    }
}
