using Application.Services;
using Application.Services.Agent;
using Application.Services.Commander;
using Application.Services.Listener;
using Application.Services.Orchestrator;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IAgentApplicationService, AgentApplicationService>();
        services.AddSingleton<IListenerApplicationService, ListenerApplicationService>();
        services.AddSingleton<ICommanderApplicationService, CommanderApplicationService>();
        services.AddSingleton<IOrchestratorApplicationService, OrchestratorApplicationService>();
        services
            .AddSingleton<IOrchestratorStateMachineConfiguratorApplicationService,
                OrchestratorStateMachineConfiguratorApplicationService>();
    }
}