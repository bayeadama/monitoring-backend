using Presentation.Orchestrator.Configurations;
using Presentation.Orchestrator.Workflows;

namespace Presentation.Orchestrator;

internal class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IWorkflowsManager _workflowsManager;
    private readonly ApplicationsConfigurations _applicationsConfigurations;
    
    public  Worker(ILogger<Worker> logger, 
            IWorkflowsManager workflowsManager, 
            ApplicationsConfigurations applicationsConfigurations)
    {
        _logger = logger;
        _workflowsManager = workflowsManager;
        _applicationsConfigurations = applicationsConfigurations;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (var application in _applicationsConfigurations.Applications)
        {
            _logger.LogInformation($"Initializing workflow for application: {application.ApplicationId}");
            await _workflowsManager.InitWorkflow(application.ApplicationId);
        }
        _logger.LogInformation("All workflows initialized successfully.");
        
        await InfiniteLoop(stoppingToken);
    }

  
    
    private async Task InfiniteLoop(CancellationToken stoppingToken)
    {
        var delay = 1000;
        while (!stoppingToken.IsCancellationRequested)
            await Task.Delay(delay, stoppingToken);
        
    }

}