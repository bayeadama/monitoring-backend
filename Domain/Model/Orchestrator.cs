using Domain.Base;
using StateMachine.Workflow;

namespace Domain.Model;

public class Orchestrator : BaseEntity
{
    public string Name { get; set; }
    public IWorkflow Workflow { get; set; }
    
    public string CurrentState => Workflow?.CurrentState ?? string.Empty;
}