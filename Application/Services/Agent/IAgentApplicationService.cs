using Application.Dto.Requests;
using Domain.Model.ValueObjects;

namespace Application.Services.Agent;

public interface IAgentApplicationService
{
    /// <summary>
    /// Initializes an agent with the given request parameters.
    /// </summary>
    /// <param name="createAgentRequest"></param>
    /// <returns></returns>
    Task<Domain.Model.Agent> InitializeAgentAsync(CreateAgentRequestDto createAgentRequest);

    /// <summary>
    /// Registers a command handler for the specified agent.
    /// </summary>
    /// <param name="agent"></param>
    /// <param name="commandHandler"></param>
    /// <returns></returns>
    Task RegisterCommandHandlerAsync(Domain.Model.Agent agent, Action<Domain.Model.Agent, Command> commandHandler);

   /// <summary>
   /// Publishes a response from the agent.
   /// </summary>
   /// <param name="agent"></param>
   /// <param name="sourceCommand"></param>
   /// <param name="agentResponse"></param>
   /// <typeparam name="T"></typeparam>
   /// <returns></returns>
    Task PublishResponseAsync<T>(Domain.Model.Agent agent, Command sourceCommand, T agentResponse);
}