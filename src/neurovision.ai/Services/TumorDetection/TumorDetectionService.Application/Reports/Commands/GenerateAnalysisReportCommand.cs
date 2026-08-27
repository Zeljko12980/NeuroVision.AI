using BuildingBlocks.Exceptions;
using BuildingBlocks.Persistence;
using MediatR;
using Microsoft.Extensions.Options;
using TumorDetectionService.Application.Common;
using TumorDetectionService.Application.Common.Interfaces;
using TumorDetectionService.Application.Common.Mapping;
using TumorDetectionService.Application.Common.Options;
using TumorDetectionService.Application.Common.Responses;
using TumorDetectionService.Domain.Enums;

namespace TumorDetectionService.Application.Reports.Commands;

public record GenerateAnalysisReportCommand(
    Guid AnalysisId,
    TumorActor Actor,
    string? DoctorName = null,
    Guid? CertificateId = null) : IRequest<AnalysisResponse>;

public class GenerateAnalysisReportCommandHandler
    : IRequestHandler<GenerateAnalysisReportCommand, AnalysisResponse>
{
    private readonly ITumorAnalysisRepository _analyses;
    private readonly IPdfReportGenerator _pdfGenerator;
    private readonly IReportStorageService _reportStorage;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOptions<PdfServiceOptions> _pdfOptions;

    public GenerateAnalysisReportCommandHandler(
        ITumorAnalysisRepository analyses,
        IPdfReportGenerator pdfGenerator,
        IReportStorageService reportStorage,
        IUnitOfWork unitOfWork,
        IOptions<PdfServiceOptions> pdfOptions)
    {
        _analyses = analyses;
        _pdfGenerator = pdfGenerator;
        _reportStorage = reportStorage;
        _unitOfWork = unitOfWork;
        _pdfOptions = pdfOptions;
    }

    public async Task<AnalysisResponse> Handle(
        GenerateAnalysisReportCommand request,
        CancellationToken cancellationToken)
    {
        var analysis = await _analyses.GetByIdWithDetailsAsync(request.AnalysisId, cancellationToken)
            ?? throw new NotFoundException($"Analysis {request.AnalysisId} not found.");
        TumorAccess.EnsureCanAccessAnalysis(request.Actor, analysis);
        TumorAccess.EnsureStaff(request.Actor);

        if (analysis.Status is not (AnalysisStatus.Completed or AnalysisStatus.Corrected))
            throw new InvalidOperationException("PDF report can only be generated for completed analyses.");

        var doctorName = string.IsNullOrWhiteSpace(request.DoctorName)
            ? _pdfOptions.Value.DefaultDoctorName
            : request.DoctorName.Trim();

        var templateData = AnalysisReportDataBuilder.Build(analysis, doctorName);
        var result = await _pdfGenerator.GenerateTumorAnalysisReportAsync(
            templateData,
            request.CertificateId,
            cancellationToken);

        if (!result.IsSigned)
            throw new InvalidOperationException("PDF report was generated without a valid digital signature.");

        var savedPath = await _reportStorage.SaveReportAsync(analysis.Id, result.PdfBytes, cancellationToken);

        analysis.SetPdfReport(savedPath);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await _analyses.GetByIdWithDetailsAsync(analysis.Id, cancellationToken);
        return AnalysisMapper.ToResponse(updated!);
    }
}
