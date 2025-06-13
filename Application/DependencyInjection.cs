using Application.Services;
using Application.Services.Agent;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IAgentApplicationService, AgentApplicationService>();
    }
}