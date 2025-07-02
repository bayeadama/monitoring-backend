using StateMachine.Configs;

namespace Application.Services.Orchestrator;

public class
    OrchestratorStateMachineConfiguratorApplicationService : IOrchestratorStateMachineConfiguratorApplicationService
{
    public Task<StateMachineConfig> InitializeStateMachineAsync(string initialStateName)
    {
        var stateMachineConfig = new StateMachineConfig
        {
            InitialStateName = initialStateName
        };
        return Task.FromResult(stateMachineConfig);
    }

    public Task<StateMachineConfig> AddStateTransitionAsync(StateMachineConfig stateMachineConfig,
        string stateName,
        StateMachineTriggerConfig triggerConfig)
    {
        if (!stateMachineConfig.StateTransitions.ContainsKey(stateName))
        {
            stateMachineConfig.StateTransitions[stateName] = new List<StateMachineTriggerConfig>();
        }

        stateMachineConfig.StateTransitions[stateName].Add(triggerConfig);
        return Task.FromResult(stateMachineConfig);
    }
}