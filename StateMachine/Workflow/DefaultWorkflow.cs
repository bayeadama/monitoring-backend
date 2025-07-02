using FluentResults;
using StateMachine.Configs;
using StateMachine.Constants;

namespace StateMachine.Workflow;

public class DefaultWorkflow : IWorkflow
{
    private string _currentState;
    private readonly StateMachineConfig _machineConfig;
    public DefaultWorkflow(StateMachineConfig machineConfig)
    {
        _machineConfig = machineConfig ?? throw new ArgumentNullException(nameof(machineConfig));
        if(machineConfig.StateTransitions == null)
            throw new ArgumentNullException($"{nameof(machineConfig)}.{nameof(machineConfig.StateTransitions)}");

        _currentState = machineConfig.InitialStateName ?? throw new ArgumentNullException($"{nameof(machineConfig)}.{nameof(machineConfig.InitialStateName)}");
    }
    
    public string CurrentState => _currentState;
    public StateMachineConfig MachineConfig => _machineConfig;
    
    public Result<string> ApplyAction(string triggerName, Dictionary<string, string> context = null)
    {
        if(!TryGetTriggerConfig(triggerName, out StateMachineTriggerConfig triggerConfig))
            return Result.Fail<string>(ErrorCodes.TriggerNotFound);
        
        //Check precondition
        if(!(triggerConfig.PreCondition?.Invoke(_currentState) ?? true))
            return Result.Fail<string>(ErrorCodes.TriggerPreconditionFailed);

        var transitionContext = new WorkflowStateTransitionContext
        {
            CurrentState = CurrentState,
            NewState = triggerConfig.NextState,
            TriggerName = triggerName
        };
        
        //Before transition
        BeforeTransitionEvent?.Invoke(transitionContext);
        if(!(triggerConfig.OnTransition?.Invoke(_currentState) ?? true))
            return Result.Fail<string>(ErrorCodes.OnTransitionFailed);
        _currentState = triggerConfig.NextState;
        //After transition
        AfterTransitionEvent?.Invoke(transitionContext);

        return Result.Ok(_currentState);
    }

    private bool TryGetTriggerConfig(string triggerName, out StateMachineTriggerConfig triggerConfig)
    {
        triggerConfig = null;
        if(!_machineConfig.StateTransitions.TryGetValue(_currentState, out List<StateMachineTriggerConfig> triggerConfigs))
            return false;

        triggerConfig = triggerConfigs.FirstOrDefault(tc => tc.TriggerName == triggerName);
        
        return triggerConfig != null;
    }

    public event BeforeTransitionEventDelegate? BeforeTransitionEvent;
    public event AfterTransitionEventDelegate? AfterTransitionEvent;
}