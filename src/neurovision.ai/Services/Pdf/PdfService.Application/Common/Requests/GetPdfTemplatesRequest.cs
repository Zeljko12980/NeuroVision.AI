namespace PdfService.Application.Common.Requests;

public sealed record GetPdfTemplatesRequest(string? Code) : PaginationRequest;
