using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using TumorDetectionService.Application.Common.Interfaces;

namespace TumorDetectionService.Infrastructure.Services;

public sealed class ScanStorageService : IScanStorageService
{
    private readonly string _root;

    public ScanStorageService(IConfiguration configuration, IWebHostEnvironment environment)
    {
        var configured = configuration["Storage:ScansPath"] ?? Path.Combine("wwwroot", "scans");
        _root = Path.IsPathRooted(configured)
            ? configured
            : Path.Combine(environment.ContentRootPath, configured);
        Directory.CreateDirectory(_root);
    }

    public async Task<string> SaveScanAsync(
        Stream content,
        string fileName,
        Guid scanId,
        CancellationToken cancellationToken = default)
    {
        var safeName = $"{scanId}_{Path.GetFileName(fileName)}";
        var fullPath = Path.Combine(_root, safeName);
        await using var fs = File.Create(fullPath);
        await content.CopyToAsync(fs, cancellationToken);
        return fullPath;
    }
}
