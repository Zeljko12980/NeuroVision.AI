namespace PdfService.Application.Common.Interfaces
{
    public interface ICertificatePasswordProtector
    {
        string Protect(string password);
        string Unprotect(string protectedPassword);
    }
}
