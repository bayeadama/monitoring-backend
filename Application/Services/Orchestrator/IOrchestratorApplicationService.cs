using StateMachine.Configs;

namespace Application.Services.Orchestrator;
using Domain.Model;

public interface IOrchestratorApplicationService
{
    /// <summary>
    /// Initializes an orchestrator with the given name.
    /// </summary>
    /// <param name="orchestratorName"></param>
    /// <returns></returns>
    Task<Orchestrator> InitializeOrchestratorAsync(string orchestratorName);
    
    /// <summary>
    /// Sets up the workflow for the given orchestrator using the provided state machine configuration.
    /// </summary>
    /// <param name="orchestrator"></param>
    /// <param name="stateMachineConfig"></param>
    /// <returns></returns>
    Task<Orchestrator> SetupWorkflow(Orchestrator orchestrator, StateMachineConfig stateMachineConfig);
}