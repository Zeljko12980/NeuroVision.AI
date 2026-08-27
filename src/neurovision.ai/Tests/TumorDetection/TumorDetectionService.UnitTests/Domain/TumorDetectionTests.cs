using TumorDetectionService.Application.Common;
using TumorDetectionService.Domain.Entities;
using TumorDetectionService.Domain.Enums;

namespace TumorDetectionService.UnitTests.Domain;

public class BrainScanTests
{
    [Fact]
    public void Create_WithValidData_SetsFields()
    {
        var patientId = Guid.NewGuid();
        var scan = BrainScan.Create(
            patientId,
            Guid.NewGuid(),
            "scan.jpg",
            "/tmp/scan.jpg",
            "image/jpeg",
            ScanType.Mri,
            1024);

        scan.PatientId.Should().Be(patientId);
        scan.FileName.Should().Be("scan.jpg");
        scan.ScanType.Should().Be(ScanType.Mri);
        scan.Analyses.Should().BeEmpty();
    }

    [Fact]
    public void Create_WithEmptyPatient_Throws()
    {
        var act = () => BrainScan.Create(
            Guid.Empty,
            Guid.NewGuid(),
            "scan.jpg",
            "/tmp/scan.jpg",
            "image/jpeg",
            ScanType.Mri,
            1024);

        act.Should().Throw<ArgumentException>().WithParameterName("patientId");
    }
}

public class TumorAnalysisTests
{
    [Fact]
    public void Create_StartsPending()
    {
        var scanId = Guid.NewGuid();
        var analysis = TumorAnalysis.Create(scanId, Guid.NewGuid());

        analysis.BrainScanId.Should().Be(scanId);
        analysis.Status.Should().Be(AnalysisStatus.Pending);
        analysis.CompletedAt.Should().BeNull();
    }

    [Fact]
    public void MarkProcessing_ThenCompleted_UpdatesStatus()
    {
        var analysis = TumorAnalysis.Create(Guid.NewGuid(), Guid.NewGuid());
        analysis.MarkProcessing();
        analysis.Status.Should().Be(AnalysisStatus.Processing);
        analysis.StartedAt.Should().NotBeNull();

        analysis.MarkCompleted(0.91, "report.json");
        analysis.Status.Should().Be(AnalysisStatus.Completed);
        analysis.OverallConfidence.Should().Be(0.91);
        analysis.ReportFilePath.Should().Be("report.json");
        analysis.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithEmptyScan_Throws()
    {
        var act = () => TumorAnalysis.Create(Guid.Empty, Guid.NewGuid());
        act.Should().Throw<ArgumentException>().WithParameterName("brainScanId");
    }
}

public class TumorAccessTests
{
    [Fact]
    public void ResolvePatientFilter_ForPatient_ForcesOwnId()
    {
        var actor = new TumorActor(Guid.Parse("22222222-2222-2222-2222-222222222222"), "Patient");
        TumorAccess.ResolvePatientFilter(actor, Guid.NewGuid()).Should().Be(actor.UserId);
    }

    [Fact]
    public void ResolvePatientFilter_ForDoctor_KeepsRequestedId()
    {
        var actor = new TumorActor(Guid.NewGuid(), "Doctor");
        var patientId = Guid.NewGuid();
        TumorAccess.ResolvePatientFilter(actor, patientId).Should().Be(patientId);
    }
}
