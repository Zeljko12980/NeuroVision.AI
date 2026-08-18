using Microsoft.AspNetCore.Hosting;
using PdfService.Application.Common.Interfaces;

namespace PdfService.Infrastructure.Services;

public sealed class CertificateStorage : ICertificateStorage
{
    private const string CertificatesFolder = "certificates";
    private const string SignaturesFolder = "signatures";

    private readonly IWebHostEnvironment _environment;

    public CertificateStorage(IWebHostEnvironment environment)
    {
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
    }

    public async Task<string> SaveAsync(
        byte[] content,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        if (content is null || content.Length == 0)
            throw new ArgumentException("The file is empty or was not provided.", nameof(content));

        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(extension))
            throw new InvalidOperationException("The file must have an extension.");

        var folder = GetCertificatesFolderPath();
        Directory.CreateDirectory(folder);

        var storedFileName = $"{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(folder, storedFileName);

        await File.WriteAllBytesAsync(fullPath, content, cancellationToken);
        return $"{CertificatesFolder}/{storedFileName}";
    }

    public async Task<byte[]> ReadAsync(
        string relativePath,
        CancellationToken cancellationToken = default)
    {
        var fullPath = GetAbsolutePath(relativePath, GetCertificatesFolderPath());
        return await File.ReadAllBytesAsync(fullPath, cancellationToken);
    }

    public Task DeleteAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return Task.CompletedTask;

        var fullPath = GetAbsolutePath(relativePath, GetCertificatesFolderPath());
        if (File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }

    public async Task<byte[]?> TryReadSignatureImageAsync(
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var fullPath = GetAbsolutePath(fileName, GetSignaturesFolderPath());
        if (!File.Exists(fullPath))
            return null;

        return await File.ReadAllBytesAsync(fullPath, cancellationToken);
    }

    private string GetCertificatesFolderPath() => GetWebRootFolder(CertificatesFolder);

    private string GetSignaturesFolderPath() => GetWebRootFolder(SignaturesFolder);

    private string GetWebRootFolder(string folderName)
    {
        var webRoot = string.IsNullOrWhiteSpace(_environment.WebRootPath)
            ? Path.Combine(_environment.ContentRootPath, "wwwroot")
            : _environment.WebRootPath;

        return Path.GetFullPath(Path.Combine(webRoot, folderName));
    }

    private static string GetAbsolutePath(string relativePath, string rootFolder)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            throw new ArgumentException("The relative path was not provided.", nameof(relativePath));

        var fileName = Path.GetFileName(relativePath.Replace('\\', '/'));
        var fullPath = Path.GetFullPath(Path.Combine(rootFolder, fileName));

        if (!fullPath.StartsWith(rootFolder, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("Invalid file path.");

        return fullPath;
    }
}
