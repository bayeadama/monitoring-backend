using Microsoft.AspNetCore.SignalR;

namespace Presentation.Orchestrator;

public class MonitoringHub : Hub
{
    public const string ReceiveStatusTransitionEvent = "ReceiveStatusTransition";
    public const string ReceiveAnalysisResultEvent = "ReceiveAnalysisResult";
    public const string WhoAmIResultEvent = "ReceiveWhoAmIResult";
    public async Task SendStatusTransition(string fromStatus, string toStatus)
    {
        await Clients.All.SendAsync(ReceiveStatusTransitionEvent, fromStatus, toStatus);
    }
}