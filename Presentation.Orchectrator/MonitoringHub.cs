using Microsoft.AspNetCore.SignalR;

namespace Presentation.Orchestrator;

public class MonitoringHub : Hub
{
    public async Task SendStatusTransition(string fromStatus, string toStatus)
    {
        await Clients.All.SendAsync("ReceiveStatusTransition", fromStatus, toStatus);
    }
}