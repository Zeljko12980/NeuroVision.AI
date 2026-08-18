using Microsoft.AspNetCore.DataProtection;
using PdfService.Application.Common.Interfaces;

namespace PdfService.Infrastructure.Services;

public sealed class CertificatePasswordProtector : ICertificatePasswordProtector
{
    private readonly IDataProtector _protector;

    public CertificatePasswordProtector(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("PdfService.CertificatePassword.v1");
    }

    public string Protect(string password) => _protector.Protect(password);

    public string Unprotect(string protectedPassword) => _protector.Unprotect(protectedPassword);
}
