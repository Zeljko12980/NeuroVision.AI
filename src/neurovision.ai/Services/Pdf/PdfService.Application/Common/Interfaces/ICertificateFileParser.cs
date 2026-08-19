namespace PdfService.Application.Common.Interfaces;

public interface ICertificateFileParser
{
    Result<ParsedCertificate> Parse(byte[] rawData, string? password);
}
