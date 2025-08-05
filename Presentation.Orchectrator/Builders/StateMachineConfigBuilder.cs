using Presentation.Orchestrator.Configurations;
using Presentation.Orchestrator.Constants;
using StateMachine.Configs;

namespace Presentation.Orchestrator;

public class StateMachineConfigBuilder : IStateMachineConfigBuilder
{

    private StateMachineConfig _config;
    
    
    public StateMachineConfigBuilder InitStandardConfig()
    {
        _config = StateMachineConfigs.GetStateMachineConfig(StateMachineType.Standard);
        return this;
    }

    public StateMachineConfigBuilder SetUpTransition(string stateName, string triggerName, Func<string, bool> transitionFunction)
    {
        if (_config == null)
            return this;
        
        if (!_config.StateTransitions.ContainsKey(stateName))
            throw new ArgumentException($"State '{stateName}' does not exist in the state machine configuration.");
        
        if (string.IsNullOrWhiteSpace(triggerName))
            throw new ArgumentException("Trigger name cannot be null or whitespace.", nameof(triggerName));
        
        if (transitionFunction == null)
            throw new ArgumentNullException(nameof(transitionFunction), "Transition function cannot be null.");
        
        //Gets the list of transitions of the given state
        var transitionConfigs = _config.StateTransitions[stateName];
        
        var transitionConfig =
            transitionConfigs.FirstOrDefault(trCfg =>
                trCfg.TriggerName == triggerName);
        
        if (transitionConfig == null)
            return this;

        transitionConfig.OnTransition = transitionFunction;

        return this;
    }

    public StateMachineConfig Build()
    {
        if (_config == null)
        {
            throw new InvalidOperationException("State machine config has not been initialized.");
        }
        
        return _config;
    }
    
   
}