using BuildingBlocks.Exceptions;
using BuildingBlocks.Persistence;
using MediatR;
using TumorDetectionService.Application.ClinicalCatalogs;
using TumorDetectionService.Application.Common;
using TumorDetectionService.Application.Common.Interfaces;
using TumorDetectionService.Domain.Entities;
using TumorDetectionService.Domain.Enums;

namespace TumorDetectionService.Application.FollowUp;

public record ClinicalFollowUpResponse(
    Guid TumorAnalysisId,
    string? GradeCode,
    string? GradeName,
    string? OperabilityCode,
    string? OperabilityName,
    string? SpreadCode,
    string? SpreadName,
    IReadOnlyList<string> TreatmentOptionCodes,
    IReadOnlyList<string> TreatmentOptionNames,
    string? SizeLocationNotes,
    string? ClinicalNotes,
    Guid? UpdatedByUserId,
    DateTime? UpdatedAt);

public record GetAnalysisClinicalFollowUpQuery(Guid AnalysisId, TumorActor Actor)
    : IRequest<ClinicalFollowUpResponse?>;

public class GetAnalysisClinicalFollowUpQueryHandler
    : IRequestHandler<GetAnalysisClinicalFollowUpQuery, ClinicalFollowUpResponse?>
{
    private readonly ITumorAnalysisRepository _analyses;
    private readonly IAnalysisClinicalFollowUpRepository _followUps;
    private readonly IClinicalCatalogRepository _catalogs;

    public GetAnalysisClinicalFollowUpQueryHandler(
        ITumorAnalysisRepository analyses,
        IAnalysisClinicalFollowUpRepository followUps,
        IClinicalCatalogRepository catalogs)
    {
        _analyses = analyses;
        _followUps = followUps;
        _catalogs = catalogs;
    }

    public async Task<ClinicalFollowUpResponse?> Handle(
        GetAnalysisClinicalFollowUpQuery request,
        CancellationToken cancellationToken)
    {
        var analysis = await _analyses.GetByIdWithDetailsAsync(request.AnalysisId, cancellationToken)
            ?? throw new NotFoundException($"Analysis {request.AnalysisId} not found.");

        TumorAccess.EnsureCanAccessAnalysis(request.Actor, analysis);

        var followUp = analysis.ClinicalFollowUp
            ?? await _followUps.GetByAnalysisIdAsync(request.AnalysisId, cancellationToken);

        if (followUp is null)
            return null;

        return await ClinicalFollowUpMapper.MapAsync(followUp, _catalogs, cancellationToken);
    }
}

internal static class ClinicalFollowUpMapper
{
    internal static async Task<ClinicalFollowUpResponse> MapAsync(
        AnalysisClinicalFollowUp followUp,
        IClinicalCatalogRepository catalogs,
        CancellationToken cancellationToken)
    {
        var treatmentCodes = followUp.GetTreatmentOptionCodes();
        var treatmentNames = new List<string>();

        foreach (var code in treatmentCodes)
        {
            var item = await catalogs.GetAsync(ClinicalCatalogCategory.TreatmentOption, code, cancellationToken);
            treatmentNames.Add(item?.Name ?? code);
        }

        var grade = followUp.GradeCode is null
            ? null
            : await catalogs.GetAsync(ClinicalCatalogCategory.Grade, followUp.GradeCode, cancellationToken);
        var operability = followUp.OperabilityCode is null
            ? null
            : await catalogs.GetAsync(ClinicalCatalogCategory.Operability, followUp.OperabilityCode, cancellationToken);
        var spread = followUp.SpreadCode is null
            ? null
            : await catalogs.GetAsync(ClinicalCatalogCategory.Spread, followUp.SpreadCode, cancellationToken);

        return new ClinicalFollowUpResponse(
            followUp.TumorAnalysisId,
            followUp.GradeCode,
            grade?.Name,
            followUp.OperabilityCode,
            operability?.Name,
            followUp.SpreadCode,
            spread?.Name,
            treatmentCodes,
            treatmentNames,
            followUp.SizeLocationNotes,
            followUp.ClinicalNotes,
            followUp.UpdatedByUserId,
            followUp.UpdatedAt);
    }
}

public record UpsertAnalysisClinicalFollowUpCommand(
    Guid AnalysisId,
    TumorActor Actor,
    string? GradeCode,
    string? OperabilityCode,
    string? SpreadCode,
    IReadOnlyList<string> TreatmentOptionCodes,
    string? SizeLocationNotes,
    string? ClinicalNotes) : IRequest<ClinicalFollowUpResponse>;

public class UpsertAnalysisClinicalFollowUpCommandHandler
    : IRequestHandler<UpsertAnalysisClinicalFollowUpCommand, ClinicalFollowUpResponse>
{
    private readonly ITumorAnalysisRepository _analyses;
    private readonly IAnalysisClinicalFollowUpRepository _followUps;
    private readonly IClinicalCatalogRepository _catalogs;
    private readonly IUnitOfWork _unitOfWork;

    public UpsertAnalysisClinicalFollowUpCommandHandler(
        ITumorAnalysisRepository analyses,
        IAnalysisClinicalFollowUpRepository followUps,
        IClinicalCatalogRepository catalogs,
        IUnitOfWork unitOfWork)
    {
        _analyses = analyses;
        _followUps = followUps;
        _catalogs = catalogs;
        _unitOfWork = unitOfWork;
    }

    public async Task<ClinicalFollowUpResponse> Handle(
        UpsertAnalysisClinicalFollowUpCommand request,
        CancellationToken cancellationToken)
    {
        TumorAccess.EnsureStaff(request.Actor);

        var analysis = await _analyses.GetByIdWithDetailsAsync(request.AnalysisId, cancellationToken)
            ?? throw new NotFoundException($"Analysis {request.AnalysisId} not found.");

        TumorAccess.EnsureCanAccessAnalysis(request.Actor, analysis);

        if (analysis.Status is not Domain.Enums.AnalysisStatus.Completed
            and not Domain.Enums.AnalysisStatus.Corrected)
        {
            throw new BadRequestException("Clinical follow-up can only be saved for completed analyses.");
        }

        await ValidateCodeAsync(ClinicalCatalogCategory.Grade, request.GradeCode, cancellationToken);
        await ValidateCodeAsync(ClinicalCatalogCategory.Operability, request.OperabilityCode, cancellationToken);
        await ValidateCodeAsync(ClinicalCatalogCategory.Spread, request.SpreadCode, cancellationToken);

        foreach (var code in request.TreatmentOptionCodes)
            await ValidateCodeAsync(ClinicalCatalogCategory.TreatmentOption, code, cancellationToken);

        var followUp = analysis.ClinicalFollowUp
            ?? await _followUps.GetByAnalysisIdAsync(request.AnalysisId, cancellationToken);

        if (followUp is null)
        {
            followUp = AnalysisClinicalFollowUp.Create(request.AnalysisId, request.Actor.UserId);
            followUp.Update(
                request.GradeCode,
                request.OperabilityCode,
                request.SpreadCode,
                request.TreatmentOptionCodes,
                request.SizeLocationNotes,
                request.ClinicalNotes,
                request.Actor.UserId);
            analysis.AttachClinicalFollowUp(followUp);
            await _followUps.AddAsync(followUp, cancellationToken);
        }
        else
        {
            followUp.Update(
                request.GradeCode,
                request.OperabilityCode,
                request.SpreadCode,
                request.TreatmentOptionCodes,
                request.SizeLocationNotes,
                request.ClinicalNotes,
                request.Actor.UserId);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await ClinicalFollowUpMapper.MapAsync(followUp, _catalogs, cancellationToken);
    }

    private async Task ValidateCodeAsync(
        ClinicalCatalogCategory category,
        string? code,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(code))
            return;

        if (await _catalogs.GetAsync(category, code, cancellationToken) is null)
            throw new BadRequestException($"Unknown catalog code '{code}' for {category}.");
    }
}
