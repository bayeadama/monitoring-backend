using Application.Services.Commander;
using Application.Services.Orchestrator;
using Domain;
using Domain.Model;
using Domain.Model.ValueObjects;
using Microsoft.AspNetCore.SignalR;
using Presentation.Orchestrator.Constants;
using StateMachine.Configs;

namespace Presentation.Orchestrator;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IOrchestratorApplicationService _orchestratorApplicationService;
    private readonly IOrchestratorStateMachineConfiguratorApplicationService _orchestratorStateMachineConfiguratorApplicationService;
    private Domain.Model.Orchestrator _orchestrator;
    private readonly StateMachineConfigBuilder _stateMachineConfigBuilder;
    private readonly ICommanderApplicationService _commanderApplicationService;
    private Commander _commander;
    private IHubContext<MonitoringHub> _monitoringHub;
    
    public  Worker(ILogger<Worker> logger, IOrchestratorApplicationService orchestratorApplicationService, IOrchestratorStateMachineConfiguratorApplicationService orchestratorStateMachineConfiguratorApplicationService, StateMachineConfigBuilder stateMachineConfigBuilder, ICommanderApplicationService commanderApplicationService, IHubContext<MonitoringHub> monitoringHub)
    {
        _logger = logger;
        _orchestratorApplicationService = orchestratorApplicationService;
        _orchestratorStateMachineConfiguratorApplicationService = orchestratorStateMachineConfiguratorApplicationService;
        _stateMachineConfigBuilder = stateMachineConfigBuilder;
        _commanderApplicationService = commanderApplicationService;
        _monitoringHub = monitoringHub;

        InitOrchestrator().GetAwaiter().GetResult();;
    }

    private async Task InitOrchestrator()
    {
        var stateMachineConfig = InitializeStateMachine();
        
        _orchestrator = await _orchestratorApplicationService.InitializeOrchestratorAsync("MyOrchestrator");
        _orchestrator = await _orchestratorApplicationService.SetupWorkflow(_orchestrator, stateMachineConfig);

        _commander = await _commanderApplicationService.InitializeCommanderAsync("Commander");
        
        _orchestrator.Workflow.AfterTransitionEvent += async (context) =>
        {
            _logger.LogInformation($"Transitioned from {context.CurrentState} to {context.NewState}" +
                                   $" using trigger {context.TriggerName}");

            await _monitoringHub.Clients.All.SendAsync("ReceiveStatusTransition", context.CurrentState, context.NewState);
            
            switch (context.NewState)
            {
                case WorkflowStates.WhoAmIRequestedState:
                    await PublishWhoAmICommand();
                    ProgrammedTask.ExecuteAsync(() =>
                    {
                        _orchestrator.Workflow.ApplyAction(TriggerNames.WhoAmiCompletedAction);
                    }, 5000);
                    break;
                
                case WorkflowStates.WhoAmICompletedState:
                   
                    ProgrammedTask.ExecuteAsync(async () =>
                    {
                        await PublishMonitoringCommand();
                        _orchestrator.Workflow.ApplyAction(TriggerNames.RequestAnalysisAction);
                    }, 5000);
                    break;
                
                case WorkflowStates.AnalysisRequestedState:
                    ProgrammedTask.ExecuteAsync(() =>
                    {
                        _orchestrator.Workflow.ApplyAction(TriggerNames.AnalysisCompletedAction);
                    }, 5000);
                    break;
                
                case WorkflowStates.AnalysisCompletedState:
                    ProgrammedTask.ExecuteAsync(() =>
                    {
                        _orchestrator.Workflow.ApplyAction(TriggerNames.RequestWhoAmiAction);
                    }, 5000);
                    break;
            }
        };
        
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ProgrammedTask.ExecuteAsync(() =>
        {
            _orchestrator.Workflow.ApplyAction(TriggerNames.RequestWhoAmiAction);
        }, 5000);
        await WorkflowLoop(stoppingToken);
    }
    
    private async Task PublishWhoAmICommand()
    {
        Command whoAmICommand = CommandFactory.CreateWhoAmiCommand();
        await _commanderApplicationService.PublishCommandAsync(_commander, whoAmICommand, "all");
    }

    private async Task PublishMonitoringCommand()
    {
        Command monitoringCommand = CommandFactory.CreateMonitoringCommand();
        await _commanderApplicationService.PublishCommandAsync(_commander, monitoringCommand, "all");
    }
    
    private async Task WorkflowLoop(CancellationToken stoppingToken)
    {
        var delay = 1000;
        while (!stoppingToken.IsCancellationRequested)
        {

            await Task.Delay(delay, stoppingToken);
        }
    }

    private StateMachineConfig InitializeStateMachine()
    {
        return _stateMachineConfigBuilder.Build(WorkflowStates.InitiatedState);
    }
}