using Application.Services.Commander;
using Application.Services.Orchestrator;
using Domain;
using Domain.Model;
using Domain.Model.ValueObjects;
using Microsoft.AspNetCore.SignalR;
using Presentation.Orchestrator.Constants;
using StateMachine.Configs;
using StateMachine.Workflow;

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
        
        _orchestrator.Workflow.AfterTransitionEvent += OnWorkflowOnAfterTransitionEvent;
        
    }

    private async void OnWorkflowOnAfterTransitionEvent(WorkflowStateTransitionContext context)
    {
        _logger.LogInformation($"Transitioned from {context.CurrentState} to {context.NewState}" + $" using trigger {context.TriggerName}");
        await _monitoringHub.Clients.All.SendAsync("ReceiveStatusTransition", context.CurrentState, context.NewState);
        await SetupStatusTasks(context);
    }

    private async Task SetupStatusTasks(WorkflowStateTransitionContext context)
    {
        switch (context.NewState)
        {
            case WorkflowStates.WhoAmIRequestedState:
                await HandleWhoAmIRequestEvent();
                break;

            case WorkflowStates.WhoAmICompletedState:
                HandleWhoAmICompletedEvent();
                break;

            case WorkflowStates.AnalysisRequestedState:
                HandleAnalysisRequestedEvent();
                break;

            case WorkflowStates.AnalysisCompletedState:
                HandleAnalysisCompletedEvent();
                break;
        }
    }

    private void HandleAnalysisCompletedEvent()
    {
        ProgrammedTask.ExecuteAsync(() => { _orchestrator.Workflow.ApplyAction(TriggerNames.RequestWhoAmiAction); }, 5000);
    }

    private void HandleAnalysisRequestedEvent()
    {
        ProgrammedTask.ExecuteAsync(() => { _orchestrator.Workflow.ApplyAction(TriggerNames.AnalysisCompletedAction); }, 5000);
    }

    private void HandleWhoAmICompletedEvent()
    {
        ProgrammedTask.ExecuteAsync(async () =>
        {
            await PublishMonitoringCommand();
            _orchestrator.Workflow.ApplyAction(TriggerNames.RequestAnalysisAction);
        }, 5000);
    }

    private async Task HandleWhoAmIRequestEvent()
    {
        await PublishWhoAmICommand();
        ProgrammedTask.ExecuteAsync(() => { _orchestrator.Workflow.ApplyAction(TriggerNames.WhoAmiCompletedAction); }, 5000);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await InitWorkflow();
        await InfiniteLoop(stoppingToken);
    }

    private async Task InitWorkflow()
    {
        await ProgrammedTask.ExecuteAsync(() =>
        {
            _orchestrator.Workflow.ApplyAction(TriggerNames.RequestWhoAmiAction);
        }, 5000);
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
    
    private async Task InfiniteLoop(CancellationToken stoppingToken)
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