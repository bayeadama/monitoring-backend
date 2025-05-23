using System.Text.Json;
using Common;
using Common.Models;

namespace Agent;

public class AgentWorker : BackgroundService
{
    private readonly ILogger<AgentWorker> _logger;
    private readonly IAgentFactory  _agentFactory;
    private readonly IAgentConfigProvider _agentConfigProvider;
    public AgentWorker(ILogger<AgentWorker> logger, IAgentFactory agentFactory, IAgentConfigProvider agentConfigProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _agentFactory = agentFactory ?? throw new ArgumentNullException(nameof(agentFactory));
        _agentConfigProvider = agentConfigProvider ?? throw new ArgumentNullException(nameof(agentConfigProvider));;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var agentConfig = _agentConfigProvider.GetConfig();
        var agent = await _agentFactory.Create(agentConfig);
        
        agent.OnCommandReceived += OnCommandReceived();
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(15000, stoppingToken);
        }
    }

    private OnCommandReceivedDelegate OnCommandReceived()
    {
        return async (sender, command) =>
        {
            _logger.LogInformation($"Command received: {command}");
            await HandleCommand(sender, command);
            
        };
    }

    async Task HandleCommand(IAgent agent, Command command)
    {
        switch (command.Name)
        {
            case CommandName.Monitoring:
            {
                await ProcessMonitoring(agent, command);
            }
                break;
        }
    }

    private static async Task ProcessMonitoring(IAgent agent, Command command)
    {
        var analysisResult = Analyse(agent);
        
        var response = new Response
        {
            FromCommand = command,
            Payload = JsonSerializer.Serialize(analysisResult)
        };
        
        
        await agent.SendResponseAsync(response);
    }

    private static StandardMonitoringResult Analyse(IAgent agent)
    {
        var analysisResult = new StandardMonitoringResult
        {
            ApplicationTrigram = agent.Id,
            ComponentName = agent.ComponentName,
            Checker = agent.Id,
            ResultOk = true,
            OkMessage = "All good"
        };
        return analysisResult;
    }
    
}