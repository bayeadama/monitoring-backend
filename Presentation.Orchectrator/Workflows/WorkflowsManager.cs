using Application.Services.Commander;
using Application.Services.Orchestrator;
using Domain;
using Domain.Model;
using Domain.Model.ValueObjects;
using Microsoft.AspNetCore.SignalR;
using Presentation.Orchestrator.Configurations;
using Presentation.Orchestrator.Constants;
using Presentation.Orchestrator.Helpers;
using StateMachine.Configs;
using StateMachine.Workflow;

namespace Presentation.Orchestrator.Workflows;

public  class WorkflowsManager : IWorkflowsManager
{
    const string MainCommanderName = "MainCommander";
    const int DefaultWaitingTime = 5000;
    
    private readonly ILogger<WorkflowsManager> _logger;
    private readonly IOrchestratorApplicationService _orchestratorApplicationService;
    private List<Domain.Model.Orchestrator> _orchestrators = new List<Domain.Model.Orchestrator>();
    private readonly Commander _commander;
    private readonly IStateMachineConfigBuilder _stateMachineConfigBuilder;
    private readonly ICommanderApplicationService _commanderApplicationService;
    private readonly IHubContext<MonitoringHub> _monitoringHub;
    private readonly ApplicationsConfigurations _applicationsConfigurations;

    public WorkflowsManager(IStateMachineConfigBuilder stateMachineConfigBuilder, ICommanderApplicationService commanderApplicationService, IOrchestratorApplicationService orchestratorApplicationService, IHubContext<MonitoringHub> monitoringHub, ILogger<WorkflowsManager> logger, ApplicationsConfigurations applicationsConfigurations)
    {
        _stateMachineConfigBuilder = stateMachineConfigBuilder ?? throw new ArgumentNullException(nameof(stateMachineConfigBuilder));
        _commanderApplicationService = commanderApplicationService;
        _orchestratorApplicationService = orchestratorApplicationService;
        _monitoringHub = monitoringHub;
        _logger = logger;
        _applicationsConfigurations = applicationsConfigurations;

        _commander = InitCommander().GetAwaiter().GetResult();
    }

    public async Task InitWorkflow(string applicationId)
    {
        StateMachineConfig stateMachineConfig = InitializeStateMachine(applicationId);
        Domain.Model.Orchestrator orchestrator = await InitOrchestrator(applicationId, stateMachineConfig);
        
        await ProgrammedTask.ExecuteAsync(() =>
        {
            orchestrator.Workflow.ApplyAction(TriggerNames.RequestWhoAmiAction);
        }, 5000);
    }

    private StateMachineConfig InitializeStateMachine(string applicationId)
    {
        var builder = _stateMachineConfigBuilder.InitStandardConfig();
        
        StateMachineConfig stateMachineConfig = builder
            .SetUpTransition(WorkflowStates.InitiatedState, TriggerNames.RequestWhoAmiAction, (currentState) =>
            {
                _logger.LogInformation("Initiated state: Request Who Am I action triggered.");
                HandleWhoAmIRequestEventAsync(applicationId).GetAwaiter().GetResult();
                return true;
            })
            .SetUpTransition(WorkflowStates.WhoAmIRequestedState, TriggerNames.WhoAmiCompletedAction, (currentState) =>
            {
                _logger.LogInformation("Who Am I requested state: Who Am I completed action triggered.");
                HandleWhoAmICompletedEventAsync(applicationId).GetAwaiter().GetResult();
                return true;
            })
            .SetUpTransition(WorkflowStates.WhoAmICompletedState, TriggerNames.RequestAnalysisAction, (currentState) =>
            {
                _logger.LogInformation("Who Am I completed state: Request Analysis action triggered.");
                HandleAnalysisRequestedEventAsync(applicationId).GetAwaiter().GetResult();
                return true;
            })
            .SetUpTransition(WorkflowStates.AnalysisRequestedState, TriggerNames.AnalysisCompletedAction, (currentState) =>
            {
                _logger.LogInformation("Analysis requested state: Analysis completed action triggered.");
                HandleAnalysisCompletedEventAsync(applicationId).GetAwaiter().GetResult();
                return true;
            })
            .SetUpTransition(WorkflowStates.AnalysisCompletedState, TriggerNames.RequestWhoAmiAction, (currentState) =>
            {
                _logger.LogInformation("Analysis completed state: Request Who Am I action triggered.");
                HandleWhoAmIRequestEventAsync(applicationId).GetAwaiter().GetResult();
                return true;
            })
            .Build();

        return stateMachineConfig;
    }
 
    private Domain.Model.Orchestrator GetOrchestrator(string applicationId)
    {
        return _orchestrators.FirstOrDefault(o => o.Name == GetOrchestratorName(applicationId)) 
               ?? throw new InvalidOperationException($"Orchestrator with name {GetOrchestratorName(applicationId)} not found.");
    }

    
    
    private async Task<Commander> InitCommander()
    {
        var commander = await _commanderApplicationService.InitializeCommanderAsync(MainCommanderName);
        await _commanderApplicationService.RegisterResponseHandlerAsync(commander, 
            (listener,response) => _ = ResponseHandler(listener, response));
        return commander;
    }

    private async Task<Domain.Model.Orchestrator> InitOrchestrator(string applicationId, 
        StateMachineConfig stateMachineConfig)
    {
        var orchestrator = await _orchestratorApplicationService.InitializeOrchestratorAsync(GetOrchestratorName(applicationId));
        orchestrator = await _orchestratorApplicationService.SetupWorkflow(orchestrator, stateMachineConfig);
        orchestrator.Workflow.AfterTransitionEvent += (WorkflowStateTransitionContext context) => OnWorkflowOnAfterTransitionEvent(context, applicationId);
        _orchestrators.Add(orchestrator);
        return orchestrator;
    }
    
    
    private async void OnWorkflowOnAfterTransitionEvent(WorkflowStateTransitionContext context, string applicationId)
    {
        _logger.LogInformation($"Transitioned from {context.CurrentState} to {context.NewState}" + $" using trigger {context.TriggerName}");
        await _monitoringHub.Clients.All.SendAsync(MonitoringHub.ReceiveStatusTransitionEvent, applicationId, context.CurrentState, context.NewState);
    }

    private async Task HandleAnalysisCompletedEventAsync(string applicationId)
    {
        var orchestrator = GetOrchestrator(applicationId);
        var waitingTime = GetStepWaitingTime(applicationId);
        await ProgrammedTask.ExecuteAsync(() =>
        {
            orchestrator.Workflow.ApplyAction(TriggerNames.RequestWhoAmiAction);
        }, waitingTime);
    }

    private async Task HandleAnalysisRequestedEventAsync(string applicationId)
    {
        var orchestrator = GetOrchestrator(applicationId);
        var waitingTime = GetStepWaitingTime(applicationId);
        await ProgrammedTask.ExecuteAsync(() =>
        {
            orchestrator.Workflow.ApplyAction(TriggerNames.AnalysisCompletedAction);
        }, waitingTime);
    }

    private async Task HandleWhoAmICompletedEventAsync(string applicationId)
    {
        var orchestrator = GetOrchestrator(applicationId);
        var waitingTime = GetStepWaitingTime(applicationId);
        await ProgrammedTask.ExecuteAsync(async () =>
        {
            await PublishMonitoringCommand(applicationId);
            orchestrator.Workflow.ApplyAction(TriggerNames.RequestAnalysisAction);
        }, waitingTime);
    }

    private async Task HandleWhoAmIRequestEventAsync(string applicationId)
    {
        var orchestrator = GetOrchestrator(applicationId);
        await PublishWhoAmICommand(applicationId);
        var waitingTime = GetStepWaitingTime(applicationId);
        await ProgrammedTask.ExecuteAsync(() =>
        {
            orchestrator.Workflow.ApplyAction(TriggerNames.WhoAmiCompletedAction);
        }, waitingTime);
    }


    private int GetStepWaitingTime(string applicationId)
    {
        var orchestrator = GetOrchestrator(applicationId);
        var applicationConfig =
            _applicationsConfigurations.Applications.FirstOrDefault(app => app.ApplicationId == applicationId);

        if (applicationConfig == null)
            return DefaultWaitingTime;

        if (!applicationConfig.Workflow.StepsWaitingTimes
                .TryGetValue(orchestrator.Workflow.CurrentState, out int waitingTime))
            return DefaultWaitingTime;
        
        return waitingTime > 0 ? waitingTime : DefaultWaitingTime;
    }


    private async Task PublishWhoAmICommand(string applicationId)
    {
        Command whoAmICommand = CommandFactory.CreateWhoAmiCommand();
        await _commanderApplicationService.PublishCommandAsync(_commander, whoAmICommand, $"{applicationId}.all");
    }

    private async Task PublishMonitoringCommand(string applicationId)
    {
        Command monitoringCommand = CommandFactory.CreateMonitoringCommand();
        await _commanderApplicationService.PublishCommandAsync(_commander, monitoringCommand, $"{applicationId}.all");
    }
    
    
    private async Task ResponseHandler(Listener listener, Response response) 
    {
        var random = new Random();
        switch (response.FromCommand.Name)
        {
            case CommandName.WhoAmI:
                await Task.Run(async () =>
                {
                    //await _monitoringHub.Clients.All.SendAsync(MonitoringHub.WhoAmIResultEvent, $"[Pre] {response.Payload}");
                    _logger.LogInformation($"WhoAmI response received: {response.Payload}");
                    await Task.Delay(random.Next(1, 8) * 1000); // Simulate delay
                    await _monitoringHub.Clients.All.SendAsync(MonitoringHub.WhoAmIResultEvent, response.Payload);

                });
                break;

            case CommandName.Monitoring:
                await Task.Run(async () =>
                {
                    _logger.LogInformation($"Monitoring response received: {response.Payload}");
                    await Task.Delay(random.Next(1, 8) * 1000); // Simulate delay
                    await _monitoringHub.Clients.All.SendAsync(MonitoringHub.ReceiveAnalysisResultEvent, response.Payload);
                });
                break;

            default:
                _logger.LogWarning($"Unknown command response received: {response.FromCommand.Name}");
                break;
        }
       
    }
    
    private static string GetOrchestratorName(string applicationId) => $"Orchestrator_{applicationId}";
    
}