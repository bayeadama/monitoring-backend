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
        //var agent = await InitAgent();
        
        //_logger.LogInformation("Agent initialized: {AgentName}", agent.Name);
        
        //await RegisterCommandHandler(agent, CommandHandler);

        await GenerateAgents();
        
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
                Category = "WebApp",
                Host = "localhost",
                Checker = "app-pool-status",
                Result = MonitoringResult.Success
            };
            
          _agentApplicationService.PublishResponseAsync(agent, command, response );
        }
        
    }
    
    private void CommandHandler(Domain.Model.Agent agent, 
        Command command,
        string group,
        string category,
        string host,
        string checker,
        MonitoringResult result)
    {
        _logger.LogInformation("Processing command: {CommandName} for agent: {AgentName}", command.Name, agent.Name);

        if (command.Name == CommandName.Monitoring)
        {
            var response = new StandardMonitoringResult
            {
                ApplicationTrigram = agent.ApplicationTrigram,
                ComponentName = agent.ComponentName,
                Category = category,
                Host = host,
                Checker = checker,
                Result = result,
                AgentId = agent.Name,
                Group = group
            };
            
            _agentApplicationService.PublishResponseAsync(agent, command, response );
        }
        
    }
    

    private async Task RegisterCommandHandler(Domain.Model.Agent agent,
        string group,
        string category,
        string host,
        string checker,
        MonitoringResult result)
    {
        await RegisterCommandHandler(agent, (a, c) =>
        {
            CommandHandler(a, c, group, category, host, checker, result);
        });
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
    
    private async Task<Domain.Model.Agent> InitAgent(string agentId, string componentName, string appTrigram)
    {
        var agent = await _agentApplicationService.InitializeAgentAsync(new CreateAgentRequestDto
        {
            AgentId = agentId,
            ApplicationTrigram = appTrigram,
            ComponentName = componentName,
            AgentType = AgentType.SemiAutonomous
        });
        return agent;
    }
    
    private async Task GenerateAgents()
    {
        const string prTrigram = "pyr";
        const string prComponentWs = "pyr.ws";

        await GeneratePgAgents();

        await GeneratePyrAgents();

        await GenerateDorAgents();

        await GenerateDinAgents();
    }

    private async Task GeneratePyrAgents()
    {
        const string prTrigram = "pyr";
        const string prComponentWs = "pyr.ws";
        
        var possibleResults = new[]
        {
            MonitoringResult.Success,
            MonitoringResult.Warning,
            MonitoringResult.Failure
        };
            
        MonitoringResult result = possibleResults[Random.Shared.Next(possibleResults.Length)];


        var pyrAgent1 = await InitAgent("pyr.agent0", prComponentWs, prTrigram);
        await RegisterCommandHandler(pyrAgent1, "All modules", "WebApp", "localhost", "app-pool-status", result);
        
        result = possibleResults[Random.Shared.Next(possibleResults.Length)];
        var pyrAgent2 = await InitAgent("pyr.agent1", prComponentWs, prTrigram);
        await RegisterCommandHandler(pyrAgent2, "All modules","Win services", "localhost", "app-pool-status", result);
    }
    
    private async Task GeneratePgAgents()
    {
        const string pgTrigram = "pbp";
        const string pgComponentBbc = "pbp.bbc";
        const string pgComponentBbs = "pbp.bbs";
        const string pgComponentPgs = "pbp.pgs";

        var configs =new []
        {
            new []{ pgComponentBbc, "webapp", "msvgvabbsconf21p", "app-pool-bbc-status"},
            new []{ pgComponentBbc, "webapp", "msvgvabbsconf22p", "app-pool-bbc-status"},
            new []{ pgComponentBbc, "webapp", "msvgvabbsconf21p", "app-pool-bbc-lq-status"},
            new []{ pgComponentBbc, "webapp", "msvgvabbsconf22p", "app-pool-bbc-lq-status"},
            new []{ pgComponentBbc, "webapp", "msvgvabbsconf21p", "app-pool-bbc-c-data-status"},
            new []{ pgComponentBbc, "webapp", "msvgvabbsconf22p", "app-pool-bbc-c-data-status"},
            
            new []{ pgComponentBbc, "webapp", "msvgvabbsconf21p", "app-site-bbc-lq-status"},
            new []{ pgComponentBbc, "webapp", "msvgvabbsconf21p", "app-site-bbc-c-data-status"},
            new []{ pgComponentBbc, "webapp", "msvgvabbsconf21p", "app-site-bbc-status"},
            
            new []{ pgComponentBbc, "webapp", "msvgvabbsconf22p", "app-site-bbc-lq-status"},
            new []{ pgComponentBbc, "webapp", "msvgvabbsconf22p", "app-site-bbc-c-data-status"},
            new []{ pgComponentBbc, "webapp", "msvgvabbsconf22p", "app-site-bbc-status"},
            
            
            new []{ pgComponentBbc, "winservices", "msvgvabbsconf21p", "ws-sx-message"},
            new []{ pgComponentBbc, "winservices", "msvgvabbsconf21p", "ws-fx-message"},
            new []{ pgComponentBbc, "winservices", "msvgvabbsconf21p", "ws-fx-timeout"},
            new []{ pgComponentBbc, "winservices", "msvgvabbsconf21p", "ws-sx-timeout"},
            
            new []{ pgComponentBbc, "winservices", "msvgvabbsconf22p", "ws-sx-message"},
            new []{ pgComponentBbc, "winservices", "msvgvabbsconf22p", "ws-fx-message"},
            new []{ pgComponentBbc, "winservices", "msvgvabbsconf22p", "ws-fx-timeout"},
            new []{ pgComponentBbc, "winservices", "msvgvabbsconf22p", "ws-sx-timeout"},
            
            new []{ pgComponentPgs, "webapp", "msvgvabbsconf21p", "app-pool-pgs-status"},
            new []{ pgComponentPgs, "webapp", "msvgvabbsconf21p", "app-pool-pgs-webapi-status"},
            new []{ pgComponentPgs, "webapp", "msvgvabbsconf21p", "app-pool-pgs-light-status"},
            new []{ pgComponentPgs, "webapp", "msvgvabbsconf21p", "app-pool-pgs-webapi-light-status"},
            
            new []{ pgComponentPgs, "webapp", "msvgvapbppgs11s", "app-pool-pgs-status"},
            new []{ pgComponentPgs, "webapp", "msvgvapbppgs11s", "app-pool-pgs-webapi-status"},
            new []{ pgComponentPgs, "webapp", "msvgvapbppgs11s", "app-pool-pgs-light-status"},
            new []{ pgComponentPgs, "webapp", "msvgvapbppgs11s", "app-pool-pgs-webapi-light-status"},
            
            new []{ pgComponentPgs, "webapp", "msvgvapbppgs12s", "app-pool-pgs-status"},
            new []{ pgComponentPgs, "webapp", "msvgvapbppgs12s", "app-pool-pgs-webapi-status"},
            new []{ pgComponentPgs, "webapp", "msvgvapbppgs12s", "app-pool-pgs-light-status"},
            new []{ pgComponentPgs, "webapp", "msvgvapbppgs12s", "app-pool-pgs-webapi-light-status"},
            
            
            new []{ pgComponentPgs, "webapp", "msvgvapbppgs11s", "app-site-pgs-status"},
            new []{ pgComponentPgs, "webapp", "msvgvapbppgs11s", "app-site-pgs-light-status"},
            
            
            new []{ pgComponentPgs, "webapp", "msvgvapbppgs12s", "app-site-pgs-status"},
            new []{ pgComponentPgs, "webapp", "msvgvapbppgs12s", "app-site-pgs-light-status"},
            
            
            
            new []{ pgComponentBbs, "webapp", "msvgvapbppgs31s", "app-pool-bbs-status"},
            new []{ pgComponentBbs, "webapp", "msvgvapbppgs31s", "app-site-bbs-status"},
            new []{ pgComponentBbs, "webapp", "msvgvapbppgs32s", "app-pool-bbs-status"},
            new []{ pgComponentBbs, "webapp", "msvgvapbppgs32s", "app-site-bbs-status"},
        };

        for (var i=0;i < configs.Length; i++)
        {
            var config = configs[i];
            var agent = await InitAgent($"pbp.agent{i}", pgComponentBbc, pgTrigram);
            var possibleResults = new[]
            {
                MonitoringResult.Success,
                MonitoringResult.Warning,
                MonitoringResult.Failure
            };
            
            MonitoringResult result = possibleResults[Random.Shared.Next(possibleResults.Length)];
            await RegisterCommandHandler(agent, 
                config[0], 
                config[1], 
                config[2], 
                config[3], 
                result);


                        
        }
    }
    
    
    private async Task GenerateDorAgents()
    {
        const string trigram = "dor";
        const string webAppComponent = "front";
        const string winSvcComponent = "back";

        var configs =new []
        {
            new []{ webAppComponent, "webapp", "dioserver1", "app-pool-status"},
            new []{ webAppComponent, "webapp", "dioserver1", "app-pool-root-status"},
            
            new []{ webAppComponent, "webapp", "dioserver1", "app-site-status"},
            
            new []{ webAppComponent, "webapp", "dioserver2", "app-pool-status"},
            new []{ webAppComponent, "webapp", "dioserver2", "app-pool-root-status"},
            
            new []{ webAppComponent, "webapp", "dioserver2", "app-site-status"},
            
            
            new []{ winSvcComponent, "winservices", "dioserver1", "listener-status"},
            new []{ winSvcComponent, "winservices", "dioserver2", "listener-status"},
        };

        for (var i=0;i < configs.Length; i++)
        {
            var config = configs[i];
            var agent = await InitAgent($"dor.agent{i}", config[0], trigram);
            var possibleResults = new[]
            {
                MonitoringResult.Success,
                MonitoringResult.Warning,
                MonitoringResult.Failure
            };
            
            MonitoringResult result = possibleResults[Random.Shared.Next(possibleResults.Length)];
            await RegisterCommandHandler(agent, 
                config[0], 
                config[1], 
                config[2], 
                config[3], 
                result);


                        
        }
    }
    
    private async Task GenerateDinAgents()
    {
        const string trigram = "din";
        const string webAppComponent = "front";
        const string winSvcComponent = "back";

        var configs =new []
        {
            new []{ webAppComponent, "webapp", "dioserver1", "app-pool-status"},
            new []{ webAppComponent, "webapp", "dioserver1", "app-pool-root-status"},
            
            new []{ webAppComponent, "webapp", "dioserver1", "app-site-status"},
            
            new []{ webAppComponent, "webapp", "dioserver2", "app-pool-status"},
            new []{ webAppComponent, "webapp", "dioserver2", "app-pool-root-status"},
            
            new []{ webAppComponent, "webapp", "dioserver2", "app-site-status"},
            
            
            new []{ winSvcComponent, "winservices", "dioserver1", "listener-status"},
            new []{ winSvcComponent, "winservices", "dioserver2", "listener-status"},
        };

        for (var i=0;i < configs.Length; i++)
        {
            var config = configs[i];
            var agent = await InitAgent($"din.agent{i}", config[0], trigram);
            var possibleResults = new[]
            {
                MonitoringResult.Success,
                MonitoringResult.Warning,
                MonitoringResult.Failure
            };
            
            MonitoringResult result = possibleResults[Random.Shared.Next(possibleResults.Length)];
            await RegisterCommandHandler(agent, 
                config[0], 
                config[1], 
                config[2], 
                config[3], 
                result);


                        
        }
    }
    
    
}