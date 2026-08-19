namespace PdfService.Application.Common.Mappings;

public static class PdfMappings
{
    public static PdfTemplateResponse ToResponse(this PdfTemplate template) =>
        new()
        {
            Id = template.Id,
            Code = template.Code,
            Name = template.Name,
            HtmlContent = template.HtmlContent,
            Version = template.Version,
            IsActive = template.IsActive,
            CreatedAt = template.CreatedAt,
            UpdatedAt = template.UpdatedAt,
            RequiresSignature = template.RequiresSignature,
            SignaturePage = template.SignaturePage,
            Fields = template.Fields.Select(field => field.ToResponse()).ToList()
        };

    public static PdfTemplateFieldResponse ToResponse(this PdfTemplateField field) =>
        new()
        {
            Id = field.Id,
            Name = field.Name,
            Type = field.Type,
            Page = field.Page,
            X = field.X,
            Y = field.Y,
            Width = field.Width,
            Height = field.Height
        };

    public static CertificateResponse ToResponse(this Certificate certificate) =>
        new()
        {
            Id = certificate.Id,
            Name = certificate.Name,
            UserId = certificate.UserId,
            Subject = certificate.Subject,
            Issuer = certificate.Issuer,
            Thumbprint = certificate.Thumbprint,
            SerialNumber = certificate.SerialNumber,
            ValidFrom = certificate.ValidFrom,
            ValidTo = certificate.ValidTo,
            FileName = certificate.FileName,
            FilePath = certificate.FilePath,
            SignatureImagePath = certificate.SignatureImagePath,
            IsDefault = certificate.IsDefault
        };
}
