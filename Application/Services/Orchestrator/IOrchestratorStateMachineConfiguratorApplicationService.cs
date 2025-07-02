using StateMachine.Configs;

namespace Application.Services.Orchestrator;

public interface IOrchestratorStateMachineConfiguratorApplicationService
{
    Task<StateMachineConfig> InitializeStateMachineAsync(string initialStateName);

    Task<StateMachineConfig> AddStateTransitionAsync(
        StateMachineConfig stateMachineConfig, 
        string stateName,
        StateMachineTriggerConfig triggerConfig);
}