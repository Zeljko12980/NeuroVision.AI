namespace MailService.API.Services.Interfaces
{
    public interface IPdfService
    {
        Task<byte[]> GenerateTwoFactorPdf(string fullName, string code);
    }
}
