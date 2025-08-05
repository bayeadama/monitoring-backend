using StateMachine.Workflow;

namespace Presentation.Orchestrator.Workflows;

public interface IWorkflowsManager
{
    Task InitWorkflow(string applicationId);
}