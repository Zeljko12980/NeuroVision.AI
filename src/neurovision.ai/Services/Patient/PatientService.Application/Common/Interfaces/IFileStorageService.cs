using Microsoft.AspNetCore.Http;

namespace PatientService.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(IFormFile file, string folder);
    Task DeleteFileAsync(string filePath);
}
