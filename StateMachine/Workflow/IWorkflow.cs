using FluentResults;
using StateMachine.Configs;

namespace StateMachine.Workflow;

public interface IWorkflow
{
    string CurrentState { get; }
    StateMachineConfig MachineConfig { get; }
    Result<string> ApplyAction(string triggerName, Dictionary<string, string> context = null);
    
    event BeforeTransitionEventDelegate BeforeTransitionEvent;
    event AfterTransitionEventDelegate AfterTransitionEvent;
}

public class WorkflowStateTransitionContext
{
    public string CurrentState { get; set; }
    public string TriggerName { get; set; }
    public string NewState { get; set; }
}

public delegate void BeforeTransitionEventDelegate(WorkflowStateTransitionContext  context);
public delegate void AfterTransitionEventDelegate(WorkflowStateTransitionContext  context);