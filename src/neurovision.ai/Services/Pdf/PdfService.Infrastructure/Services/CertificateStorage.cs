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

    public Task<string> SaveAsync(
        byte[] content,
        string fileName,
        CancellationToken cancellationToken = default) =>
        SaveToFolderAsync(content, fileName, CertificatesFolder, cancellationToken);

    public Task<string> SaveSignatureImageAsync(
        byte[] content,
        string fileName,
        CancellationToken cancellationToken = default) =>
        SaveToFolderAsync(content, fileName, SignaturesFolder, cancellationToken);

    public async Task<byte[]> ReadAsync(
        string relativePath,
        CancellationToken cancellationToken = default)
    {
        var fullPath = GetAbsolutePath(relativePath);
        return await File.ReadAllBytesAsync(fullPath, cancellationToken);
    }

    public Task DeleteAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return Task.CompletedTask;

        var fullPath = GetAbsolutePath(relativePath);
        if (File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }

    public async Task<byte[]?> TryReadSignatureImageAsync(
        string fileName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return null;

        var relativePath = fileName.Replace('\\', '/').Contains('/')
            ? fileName
            : $"{SignaturesFolder}/{fileName}";

        var fullPath = GetAbsolutePath(relativePath);
        if (!File.Exists(fullPath))
            return null;

        return await File.ReadAllBytesAsync(fullPath, cancellationToken);
    }

    private async Task<string> SaveToFolderAsync(
        byte[] content,
        string fileName,
        string folderName,
        CancellationToken cancellationToken)
    {
        if (content is null || content.Length == 0)
            throw new ArgumentException("The file is empty or was not provided.", nameof(content));

        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(extension))
            throw new InvalidOperationException("The file must have an extension.");

        var folder = GetWebRootFolder(folderName);
        Directory.CreateDirectory(folder);

        var storedFileName = $"{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(folder, storedFileName);

        await File.WriteAllBytesAsync(fullPath, content, cancellationToken);
        return $"{folderName}/{storedFileName}";
    }

    private string GetWebRootFolder(string folderName)
    {
        var webRoot = string.IsNullOrWhiteSpace(_environment.WebRootPath)
            ? Path.Combine(_environment.ContentRootPath, "wwwroot")
            : _environment.WebRootPath;

        return Path.GetFullPath(Path.Combine(webRoot, folderName));
    }

    private string GetAbsolutePath(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            throw new ArgumentException("The relative path was not provided.", nameof(relativePath));

        var normalized = relativePath.Replace('\\', '/').TrimStart('/');
        var folderName = normalized.StartsWith($"{SignaturesFolder}/", StringComparison.OrdinalIgnoreCase)
            ? SignaturesFolder
            : CertificatesFolder;

        var rootFolder = GetWebRootFolder(folderName);
        var fileName = Path.GetFileName(normalized);
        var fullPath = Path.GetFullPath(Path.Combine(rootFolder, fileName));

        if (!fullPath.StartsWith(rootFolder, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("Invalid file path.");

        return fullPath;
    }
}
