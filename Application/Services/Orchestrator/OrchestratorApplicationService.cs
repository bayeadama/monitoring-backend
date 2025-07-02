using StateMachine.Configs;
using StateMachine.Workflow;

namespace Application.Services.Orchestrator;

public class OrchestratorApplicationService : IOrchestratorApplicationService
{
    
    /// <inheritdoc cref="IOrchestratorApplicationService.InitializeOrchestratorAsync"/>
    public Task<Domain.Model.Orchestrator> InitializeOrchestratorAsync(string orchestratorName)
    {
        var orchestrator = new Domain.Model.Orchestrator
        {
            Name = orchestratorName
        };
        return Task.FromResult(orchestrator);
    }

    /// <inheritdoc cref="IOrchestratorApplicationService.SetupWorkflow"/>
    public Task<Domain.Model.Orchestrator> SetupWorkflow(Domain.Model.Orchestrator orchestrator, StateMachineConfig stateMachineConfig)
    {
        if (orchestrator == null)
        {
            throw new ArgumentNullException(nameof(orchestrator), "Orchestrator cannot be null");
        }

        if (stateMachineConfig == null)
        {
            throw new ArgumentNullException(nameof(stateMachineConfig), "StateMachineConfig cannot be null");
        }

        orchestrator.Workflow = new DefaultWorkflow(stateMachineConfig);
        return Task.FromResult(orchestrator);
    }
}