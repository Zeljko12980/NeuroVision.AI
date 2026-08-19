using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using PdfService.Application.Common.Models;
using PdfService.Domain.Entities;

namespace PdfService.Infrastructure.Services;

internal static class SignaturePlacementHelper
{
    private const float DefaultMarginX = 72f;
    private const float DefaultMarginY = 72f;
    private const float DefaultWidth = 200f;
    private const float DefaultHeight = 60f;

    public static SignaturePosition ResolveFromPdf(byte[] pdfBytes, PdfTemplate template)
    {
        var field = template.GetSignatureField();

        var width = field?.Width > 0 ? field.Width : DefaultWidth;
        var height = field?.Height > 0 ? field.Height : DefaultHeight;
        var marginX = field?.X > 0 ? field.X : DefaultMarginX;
        var marginY = field?.Y > 0 ? field.Y : DefaultMarginY;

        using var reader = new PdfReader(new MemoryStream(pdfBytes));
        using var pdfDoc = new PdfDocument(reader);

        var totalPages = pdfDoc.GetNumberOfPages();
        var targetPage = field is { Page: > 0 }
            ? Math.Min(field.Page, totalPages)
            : totalPages;

        return new SignaturePosition
        {
            Page = targetPage,
            X = marginX,
            Y = marginY,
            Width = width,
            Height = height
        };
    }

    public static Rectangle ToPageRect(SignaturePosition position) =>
        new(position.X, position.Y, position.X + position.Width, position.Y + position.Height);
}
