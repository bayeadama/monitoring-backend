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
        services.AddTransient<IAgentApplicationService, AgentApplicationService>();
        services.AddTransient<IListenerApplicationService, ListenerApplicationService>();
        services.AddTransient<ICommanderApplicationService, CommanderApplicationService>();
        services.AddTransient<IOrchestratorApplicationService, OrchestratorApplicationService>();
        services
            .AddTransient<IOrchestratorStateMachineConfiguratorApplicationService,
                OrchestratorStateMachineConfiguratorApplicationService>();
    }
}