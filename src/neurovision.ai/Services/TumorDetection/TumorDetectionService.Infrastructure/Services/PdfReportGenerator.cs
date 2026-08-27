using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PdfService.Grpc;
using TumorDetectionService.Application.Common.Interfaces;
using TumorDetectionService.Application.Common.Options;

namespace TumorDetectionService.Infrastructure.Services;

public sealed class PdfReportGenerator : IPdfReportGenerator
{
    private readonly PdfGenerator.PdfGeneratorClient _client;
    private readonly PdfServiceOptions _options;
    private readonly ILogger<PdfReportGenerator> _logger;

    public PdfReportGenerator(
        PdfGenerator.PdfGeneratorClient client,
        IOptions<PdfServiceOptions> options,
        ILogger<PdfReportGenerator> logger)
    {
        _client = client;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<PdfReportGenerationResult> GenerateTumorAnalysisReportAsync(
        Dictionary<string, string> templateData,
        Guid? certificateId = null,
        CancellationToken cancellationToken = default)
    {
        var signingCertificateId = certificateId ?? _options.DefaultCertificateId;
        if (signingCertificateId is null || signingCertificateId == Guid.Empty)
        {
            throw new InvalidOperationException(
                "A signing certificate is required. Configure PdfService:DefaultCertificateId or upload a default certificate in PDF admin.");
        }

        var request = new GeneratePdfRequest
        {
            TemplateCode = TumorReportTemplates.AnalysisReport,
            CertificateId = signingCertificateId.Value.ToString(),
        };

        foreach (var (key, value) in templateData)
        {
            request.Placeholders.Add(key, value);
        }

        _logger.LogInformation(
            "Generating signed tumor report via gRPC. Template={Template}, CertificateId={CertificateId}",
            TumorReportTemplates.AnalysisReport,
            signingCertificateId);

        var response = await _client.GeneratePdfAsync(
            request,
            deadline: DateTime.UtcNow.AddSeconds(_options.TimeoutSeconds),
            cancellationToken: cancellationToken);

        if (!response.Success)
        {
            _logger.LogError("PDF gRPC generation failed: {Message}", response.Message);
            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(response.Message)
                    ? "Failed to generate PDF report from PDF service."
                    : response.Message);
        }

        if (response.Pdf is null || response.Pdf.Length == 0)
            throw new InvalidOperationException("PDF service returned empty PDF bytes.");

        if (!response.IsSigned)
        {
            throw new InvalidOperationException(
                "PDF report was generated without a valid digital signature. " +
                "Ensure PdfService is running, the default certificate is seeded, and signature.png exists.");
        }

        return new PdfReportGenerationResult(response.Pdf.ToByteArray(), IsSigned: true);
    }
}

public sealed class ReportStorageService : IReportStorageService
{
    private readonly string _root;

    public ReportStorageService(
        IConfiguration configuration,
        Microsoft.AspNetCore.Hosting.IWebHostEnvironment environment)
    {
        var configured = configuration["Storage:ReportsPath"] ?? Path.Combine("wwwroot", "reports");
        _root = Path.IsPathRooted(configured)
            ? configured
            : Path.Combine(environment.ContentRootPath, configured);
        Directory.CreateDirectory(_root);
    }

    public async Task<string> SaveReportAsync(
        Guid analysisId,
        byte[] pdfBytes,
        CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_root, $"{analysisId}.pdf");
        await File.WriteAllBytesAsync(fullPath, pdfBytes, cancellationToken);
        return fullPath;
    }

    public string? GetReportPath(Guid analysisId)
    {
        var fullPath = Path.Combine(_root, $"{analysisId}.pdf");
        return File.Exists(fullPath) ? fullPath : null;
    }
}

public static class PdfReportServiceRegistration
{
    public static IServiceCollection AddPdfReportServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<PdfServiceOptions>(configuration.GetSection("PdfService"));

        var grpcClientBuilder = services.AddGrpcClient<PdfGenerator.PdfGeneratorClient>((sp, options) =>
        {
            var pdfOptions = sp.GetRequiredService<IOptions<PdfServiceOptions>>().Value;
            var config = sp.GetRequiredService<IConfiguration>();
            var address = PdfGrpcAddressResolver.Resolve(config, pdfOptions);
            options.Address = address;

            var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("PdfGrpcClient");
            logger.LogInformation("PDF gRPC client configured for {Address}", address);
        })
        .ConfigureChannel(channel =>
        {
            channel.MaxReceiveMessageSize = 64 * 1024 * 1024;
            channel.MaxSendMessageSize = 64 * 1024 * 1024;
        })
        .ConfigurePrimaryHttpMessageHandler(sp =>
        {
            var env = sp.GetRequiredService<IHostEnvironment>();
            var handler = new HttpClientHandler();
            if (env.IsDevelopment())
            {
                handler.ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            }

            return handler;
        });

        grpcClientBuilder.AddStandardResilienceHandler(options =>
        {
            var timeoutSeconds = configuration.GetValue("PdfService:TimeoutSeconds", 180);
            var timeout = TimeSpan.FromSeconds(Math.Clamp(timeoutSeconds, 30, 600));

            options.TotalRequestTimeout.Timeout = timeout;
            options.AttemptTimeout.Timeout = timeout;
            options.Retry.MaxRetryAttempts = 1;
            options.CircuitBreaker.SamplingDuration = timeout * 2;
        });
        services.AddScoped<IPdfReportGenerator, PdfReportGenerator>();
        services.AddSingleton<IReportStorageService, ReportStorageService>();
        return services;
    }
}
