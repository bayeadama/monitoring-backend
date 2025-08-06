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
            var possibleResults = new[]
            {
                MonitoringResult.Success,
                MonitoringResult.Warning,
                MonitoringResult.Failure
            };
            
            MonitoringResult mr = possibleResults[Random.Shared.Next(possibleResults.Length)];
            
            var response = new StandardMonitoringResult
            {
                ApplicationTrigram = agent.ApplicationTrigram,
                ComponentName = agent.ComponentName,
                Category = category,
                Host = host,
                Checker = checker,
                Result = mr,
                AgentId = agent.Name,
                Group = group
            };
            
            _agentApplicationService.PublishResponseAsync(agent, command, response );
        }
        
    }

    private async Task PublishCounterResponse(Domain.Model.Agent agent,
        string group,
        string category,
        string host,
        string checker,
        int[] counterValues)
    {
         
        int mr = counterValues[Random.Shared.Next(counterValues.Length)];
        var response = new CounterMonitoringResult
        {
            ApplicationTrigram = agent.ApplicationTrigram,
            ComponentName = agent.ComponentName,
            Category = category,
            Host = host,
            Checker = checker,
            Result = mr,
            AgentId = agent.Name,
            Group = group
        };
            
        _agentApplicationService.PublishResponseAsync(agent, response );
    }
    
    private async Task PublishResourseResponse(Domain.Model.Agent agent,
        string group,
        string category,
        string host,
        string checker,
        int[] memValues,
        int maxMemorySize,
        int[] diskValues,
        int maxDiskSize,
        string title)
    {
         
        int mv = memValues[Random.Shared.Next(memValues.Length)];
        int dv = diskValues[Random.Shared.Next(diskValues.Length)];
        
        var response = new MonitoringResult<ResourceMonitoringResult>
        {
            ApplicationTrigram = agent.ApplicationTrigram,
            ComponentName = agent.ComponentName,
            Category = category,
            Host = host,
            Checker = checker,
            Result = new ResourceMonitoringResult
            {
                TotalDiskSpace = maxDiskSize,
                UsedDiskSpace = dv,
                TotalMemory = maxMemorySize,
                UsedMemory = mv,
                MachineName = host,
                Title = title
            },
            AgentId = agent.Name,
            Group = group
        };
            
        _agentApplicationService.PublishResponseAsync(agent, response );
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

        await GeneratePgAppUsersCoutersAgents();
        
        GeneratePgAppResourceMonitoringAgents();
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

    private async Task GeneratePgAppResourceMonitoringAgents()
    {
        const string trigram = "pbp";
        await GenerateGvaPgResourceMonitoringAgents(trigram);
        await GenerateLdnPgResourceMonitoringAgents(trigram);
        await GenerateLuxPgResourceMonitoringAgents(trigram);
        await GenerateJrsPgResourceMonitoringAgents(trigram);
        await GenerateMcmPgResourceMonitoringAgents(trigram);
        await GenerateMilPgResourceMonitoringAgents(trigram);
    }

    private async Task GenerateMilPgResourceMonitoringAgents(string trigram)
    {
        var agent1 = await InitAgent("resources.pbp.mil.agent1","pbp.mil.resources",trigram );
        
        RunInfiniteLoop(() =>
        {
            PublishResourseResponse(agent1,
                "MIL resources",
                "resources.monitoring",
                "pgs.mil.host1",
                "pgs.mil.resources.monitoring", 
                new[] { 70, 75, 80, 96 }, 100,
                new[] { 70, 75, 80, 96 }, 100,
                "FRONT");
        }, 5000, CancellationToken.None);
        
        var agent2 = await InitAgent("resources.pbp.mil.agent2","pbp.mil.resources",trigram );
        
        RunInfiniteLoop(() =>
        {
            PublishResourseResponse(agent2,
                "MIL resources",
                "resources.monitoring",
                "pgs.mil.host2",
                "bbs.mil.resources.monitoring", 
                new[] { 70, 75, 80, 96 }, 100,
                new[] { 70, 75, 80, 96 }, 100,
                "BBS");
        }, 5000, CancellationToken.None);
    }
    private async Task GenerateMcmPgResourceMonitoringAgents(string trigram)
    {
        var agent1 = await InitAgent("resources.pbp.mcm.agent1","pbp.mcm.resources",trigram );
        
        RunInfiniteLoop(() =>
        {
            PublishResourseResponse(agent1,
                "MCM resources",
                "resources.monitoring",
                "pgs.mcm.host1",
                "pgs.mcm.resources.monitoring", 
                new[] { 70, 75, 80, 96 }, 100,
                new[] { 70, 75, 80, 96 }, 100,
                "FRONT");
        }, 5000, CancellationToken.None);
        
        var agent2 = await InitAgent("resources.pbp.mcm.agent2","pbp.mcm.resources",trigram );
        
        RunInfiniteLoop(() =>
        {
            PublishResourseResponse(agent2,
                "MCM resources",
                "resources.monitoring",
                "pgs.mcm.host2",
                "bbs.mcm.resources.monitoring", 
                new[] { 70, 75, 80, 96 }, 100,
                new[] { 70, 75, 80, 96 }, 100,
                "BBS");
        }, 5000, CancellationToken.None);
    }
    
    private async Task GenerateJrsPgResourceMonitoringAgents(string trigram)
    {
        var agent1 = await InitAgent("resources.pbp.jrs.agent1","pbp.jrs.resources",trigram );
        
        RunInfiniteLoop(() =>
        {
            PublishResourseResponse(agent1,
                "JRS resources",
                "resources.monitoring",
                "pgs.jrs.host1",
                "pgs.jrs.resources.monitoring", 
                new[] { 70, 75, 80, 96 }, 100,
                new[] { 70, 75, 80, 96 }, 100,
                "FRONT");
        }, 5000, CancellationToken.None);
        
        var agent2 = await InitAgent("resources.pbp.jrs.agent2","pbp.jrs.resources",trigram );
        
        RunInfiniteLoop(() =>
        {
            PublishResourseResponse(agent2,
                "JRS resources",
                "resources.monitoring",
                "pgs.jrs.host2",
                "bbs.jrs.resources.monitoring", 
                new[] { 70, 75, 80, 96 }, 100,
                new[] { 70, 75, 80, 96 }, 100,
                "BBS");
        }, 5000, CancellationToken.None);
    }
    
    private async Task GenerateLuxPgResourceMonitoringAgents(string trigram)
    {
        var agent1 = await InitAgent("resources.pbp.lux.agent1","pbp.lux.resources",trigram );
        
        RunInfiniteLoop(() =>
        {
            PublishResourseResponse(agent1,
                "LUX resources",
                "resources.monitoring",
                "pgs.lux.host1",
                "pgs.lux.resources.monitoring", 
                new[] { 70, 75, 80, 96 }, 100,
                new[] { 70, 75, 80, 96 }, 100,
                "FRONT");
        }, 5000, CancellationToken.None);
        
        var agent2 = await InitAgent("resources.pbp.lux.agent2","pbp.lux.resources",trigram );
        
        RunInfiniteLoop(() =>
        {
            PublishResourseResponse(agent2,
                "LUX resources",
                "resources.monitoring",
                "pgs.lux.host2",
                "bbs.lux.resources.monitoring", 
                new[] { 70, 75, 80, 96 }, 100,
                new[] { 70, 75, 80, 96 }, 100,
                "BBS");
        }, 5000, CancellationToken.None);
    }
    
    private async Task GenerateLdnPgResourceMonitoringAgents(string trigram)
    {
        var agent1 = await InitAgent("resources.pbp.ldn.agent1","pbp.ldn.resources",trigram );
        
        RunInfiniteLoop(() =>
        {
            PublishResourseResponse(agent1,
                "LDN resources",
                "resources.monitoring",
                "pgs.ldn.host1",
                "pgs.ldn.resources.monitoring", 
                new[] { 70, 75, 80, 96 }, 100,
                new[] { 70, 75, 80, 96 }, 100,
                "FRONT");
        }, 5000, CancellationToken.None);
        
        var agent2 = await InitAgent("resources.pbp.ldn.agent2","pbp.ldn.resources",trigram );
        
        RunInfiniteLoop(() =>
        {
            PublishResourseResponse(agent2,
                "LDN resources",
                "resources.monitoring",
                "pgs.ldn.host2",
                "bbs.ldn.resources.monitoring", 
                new[] { 70, 75, 80, 96 }, 100,
                new[] { 70, 75, 80, 96 }, 100,
                "BBS");
        }, 5000, CancellationToken.None);
    }
    
    private async Task GenerateGvaPgResourceMonitoringAgents(string trigram)
    {
        var gvaAgentPgs1 = await InitAgent("resources.pbp.gva.agent1","pbp.gva.resources",trigram );
        
        RunInfiniteLoop(() =>
        {
            PublishResourseResponse(gvaAgentPgs1,
                "GVA resources",
                "resources.monitoring",
                "pgs.gva.host1",
                "pgs-1.gva.resources.monitoring", 
                new[] { 70, 75, 80, 96 }, 100,
                new[] { 70, 75, 80, 96 }, 100,
                "FRONT1");
        }, 5000, CancellationToken.None);
        
        var gvaAgentPgs2 = await InitAgent("resources.pbp.gva.agent2","pbp.gva.resources",trigram );
        
        RunInfiniteLoop(() =>
        {
            PublishResourseResponse(gvaAgentPgs2,
                "GVA resources",
                "resources.monitoring",
                "pgs.gva.host2",
                "pgs-2.gva.resources.monitoring", 
                new[] { 70, 75, 80, 96 }, 100,
                new[] { 70, 75, 80, 96 }, 100,
                "FRONT2");
        }, 5000, CancellationToken.None);
        
        var gvaAgentBbs1 = await InitAgent("resources.pbp.gva.agent3","pbp.gva.resources",trigram );
        
        RunInfiniteLoop(() =>
        {
            PublishResourseResponse(gvaAgentBbs1,
                "GVA resources",
                "resources.monitoring",
                "bbs.gva.host1",
                "bbs-1.gva.resources.monitoring", 
                new[] { 70, 75, 80, 96 }, 100,
                new[] { 70, 75, 80, 96 }, 100,
                "BBS1");
        }, 5000, CancellationToken.None);
        
        var gvaAgentBbs2 = await InitAgent("resources.pbp.gva.agent4","pbp.gva.resources",trigram );
        
        RunInfiniteLoop(() =>
        {
            PublishResourseResponse(gvaAgentBbs2,
                "GVA resources",
                "resources.monitoring",
                "bbs.gva.host2",
                "bbs-2.gva.resources.monitoring", 
                new[] { 70, 75, 80, 96 }, 100,
                new[] { 70, 75, 80, 96 }, 100,
                "BBS2");
        }, 5000, CancellationToken.None);
    }


    private async Task GeneratePgAppUsersCoutersAgents()
    {
        const string trigram = "pbp";
        var gvaAgent = await InitAgent("counter.pbp.gva.agent","pbp.gva.users",trigram );
        
        RunInfiniteLoop(() =>
        {
            PublishCounterResponse(gvaAgent,
                "GVA users",
                "stats.counter",
                "localhost",
                "pgs-1.gva.users.stats.counter",
                new[] { 450,460, 444, 500 });
        }, 5000, CancellationToken.None);
        
        var jrsAgent = await InitAgent("counter.pbp.jrs.agent","pbp.jrs.users",trigram );
        
        RunInfiniteLoop(() =>
        {
            PublishCounterResponse(jrsAgent,
                "JRS users",
                "stats.counter",
                "localhost",
                "pgs.jrs.users.stats.counter",
                new[] { 100,110, 120, 150 });
        }, 5000, CancellationToken.None);
        
        var ldnAgent = await InitAgent("counter.pbp.ldn.agent","pbp.ldn.users",trigram );
        
        RunInfiniteLoop(() =>
        {
            PublishCounterResponse(ldnAgent,
                "LDN users",
                "stats.counter",
                "localhost",
                "pgs.ldn.users.stats.counter",
                new[] { 100,110, 120, 150 });
        }, 5000, CancellationToken.None);
        

        
        var mcmAgent = await InitAgent("counter.pbp.mcm.agent","pbp.mcm.users",trigram );
        
        RunInfiniteLoop(() =>
        {
            PublishCounterResponse(mcmAgent,
                "MCM users",
                "stats.counter",
                "localhost",
                "pgs.mcm.users.stats.counter",
                new[] { 100,110, 120, 150 });
        }, 5000, CancellationToken.None);
        
        var milAgent = await InitAgent("counter.pbp.mil.agent","pbp.mil.users",trigram );
        
        RunInfiniteLoop(() =>
        {
            PublishCounterResponse(milAgent,
                "MIL users",
                "stats.counter",
                "localhost",
                "pgs.mil.users.stats.counter",
                new[] { 100,110, 120, 150 });
        }, 5000, CancellationToken.None);
        
        var luxAgent = await InitAgent("counter.pbp.lux.agent","pbp.lux.users",trigram );
        
        RunInfiniteLoop(() =>
        {
            PublishCounterResponse(luxAgent,
                "LUX users",
                "stats.counter",
                "localhost",
                "pgs.lux.users.stats.counter",
                new[] { 100,110, 120, 150 });
        }, 5000, CancellationToken.None);
        
        
    }
    
    
    private Task RunInfiniteLoop(Action action, int waitingTime, CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                action();
                await Task.Delay(waitingTime, stoppingToken);
            }
        }, stoppingToken);
    }
    
}

public class ResourceMonitoringResult
{
    public int UsedMemory { get; set; }
    public int TotalMemory { get; set; }
    
    public int UsedDiskSpace { get; set; }
    public int TotalDiskSpace { get; set; }

    public string MachineName { get; set; }

    public string Title { get; set; }
}