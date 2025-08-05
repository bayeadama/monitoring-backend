using StateMachine.Configs;

namespace Presentation.Orchestrator;

public interface IStateMachineConfigBuilder
{
    StateMachineConfigBuilder InitStandardConfig();
    
    StateMachineConfigBuilder SetUpTransition(string stateName, 
        string triggerName, 
        Func<string, bool> transitionFunction);

    StateMachineConfig Build();
}