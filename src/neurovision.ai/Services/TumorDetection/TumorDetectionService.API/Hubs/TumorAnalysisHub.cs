using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace TumorDetectionService.API.Hubs;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class TumorAnalysisHub : Hub
{
    public Task JoinAnalysis(Guid analysisId) =>
        Groups.AddToGroupAsync(Context.ConnectionId, AnalysisGroup(analysisId));

    public Task LeaveAnalysis(Guid analysisId) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, AnalysisGroup(analysisId));

    public Task JoinPatient(Guid patientId) =>
        Groups.AddToGroupAsync(Context.ConnectionId, PatientGroup(patientId));

    public Task LeavePatient(Guid patientId) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, PatientGroup(patientId));

    public Task JoinAllAnalyses() =>
        Groups.AddToGroupAsync(Context.ConnectionId, AllAnalysesGroup);

    public Task LeaveAllAnalyses() =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, AllAnalysesGroup);

    internal static string AnalysisGroup(Guid analysisId) => $"analysis:{analysisId}";

    internal static string PatientGroup(Guid patientId) => $"patient:{patientId}";

    internal const string AllAnalysesGroup = "analyses:all";
}
