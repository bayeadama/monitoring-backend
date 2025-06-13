using Application.Dto.Requests;
using Application.Services;
using Application.Services.Agent;
using Domain.Model;
using Domain.Model.ValueObjects;

namespace Presentation.Agent;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IAgentApplicationService _agentApplicationService;

    public Worker(ILogger<Worker> logger, IAgentApplicationService agentApplicationService)
    {
        _logger = logger;
        _agentApplicationService = agentApplicationService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var agent = await InitAgent();
        
        _logger.LogInformation("Agent initialized: {AgentName}", agent.Name);
        
        await RegisterCommandHandler(agent, CommandHandler);
        
        await RunInfinitely(stoppingToken);
    }

    private static async Task RunInfinitely(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task RegisterCommandHandler(Domain.Model.Agent agent, Action<Domain.Model.Agent,Command> commandHandler)
    {
        await _agentApplicationService.RegisterCommandHandlerAsync(agent, commandHandler);
    }

    private void CommandHandler(Domain.Model.Agent agent, Command command)
    {
        _logger.LogInformation("Processing command: {CommandName} for agent: {AgentName}", command.Name, agent.Name);

        if (command.Name == CommandName.Monitoring)
        {
            var response = new StandardMonitoringResult
            {
                ApplicationTrigram = agent.ApplicationTrigram,
                ComponentName = agent.ComponentName,
                Checker = "server-health-check",
                ResultOk = true
            };
            
          _agentApplicationService.PublishResponseAsync(agent, command, response );
        }
        
    }
    
    private async Task<Domain.Model.Agent> InitAgent()
    {
        var agent = await _agentApplicationService.InitializeAgentAsync(new CreateAgentRequestDto
        {
            AgentId = "Agent1",
            ApplicationTrigram = "APP",
            ComponentName = "Component1",
            AgentType = AgentType.SemiAutonomous
        });
        return agent;
    }
}