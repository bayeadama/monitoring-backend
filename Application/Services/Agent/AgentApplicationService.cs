using System.Text.Json;
using Application.Dto.Requests;
using Application.Interfaces;
using Domain.Model;
using Domain.Model.ValueObjects;

namespace Application.Services.Agent;

public class AgentApplicationService : IAgentApplicationService
{
    private readonly IMessagePublisher<Domain.Model.Agent, Response> _responseMessagePublisher;
    private readonly IMessageReceiver<Domain.Model.Agent, Command> _commandReceiver;

    public AgentApplicationService(IMessagePublisher<Domain.Model.Agent, Response> responseMessagePublisher,
        IMessageReceiver<Domain.Model.Agent, Command> commandReceiver)
    {
        _responseMessagePublisher = responseMessagePublisher ??
                                    throw new ArgumentNullException(nameof(responseMessagePublisher));
        _commandReceiver = commandReceiver ?? throw new ArgumentNullException(nameof(commandReceiver));
    }
    
    
    /// <inheritdoc />
    public Task<Domain.Model.Agent> InitializeAgentAsync(CreateAgentRequestDto createAgentRequest)
    {
        if (createAgentRequest == null)
        {
            throw new ArgumentNullException(nameof(createAgentRequest));
        }

        Domain.Model.Agent agent = InitAgent(createAgentRequest);
       
        return Task.FromResult(agent);
    }

    /// <inheritdoc />
    public async Task RegisterCommandHandlerAsync(Domain.Model.Agent agent, Action<Domain.Model.Agent,Command> commandHandler)
    {
        if(agent == null)
            throw new ArgumentNullException(nameof(agent));
        
        if(agent.AgentType == AgentType.Autonomous)
            throw new InvalidOperationException("Autonomous agents cannot register command handlers.");
        
        if (commandHandler == null)
            throw new ArgumentNullException(nameof(commandHandler));
        
        await _commandReceiver.RegisterHandlerAsync(agent, (command) => ProcessCommandAsync(agent, command, commandHandler));
    }

    /// <inheritdoc />
    public Task PublishResponseAsync<T>(Domain.Model.Agent agent, Command sourceCommand, T agentResponse)
    {
        if (agent == null)
            throw new ArgumentNullException(nameof(agent));
        
        if (sourceCommand == null)
            throw new ArgumentNullException(nameof(sourceCommand));
        
        if (agentResponse == null)
            throw new ArgumentNullException(nameof(agentResponse));

        var response = new Response
        {
            ApplicationTrigram = agent.ApplicationTrigram,
            FromAgent = agent.Name,
            FromCommand = sourceCommand,
            Payload = JsonSerializer.Serialize(agentResponse)
        };

        return _responseMessagePublisher.PublishAsync(agent, response);
    }
    
    public Task PublishResponseAsync<T>(Domain.Model.Agent agent, T agentResponse)
    {
        if (agent == null)
            throw new ArgumentNullException(nameof(agent));
        
        if (agentResponse == null)
            throw new ArgumentNullException(nameof(agentResponse));

        var response = new Response
        {
            ApplicationTrigram = agent.ApplicationTrigram,
            FromAgent = agent.Name,
            Payload = JsonSerializer.Serialize(agentResponse)
        };

        return _responseMessagePublisher.PublishAsync(agent, response);
    }

    private Domain.Model.Agent InitAgent(CreateAgentRequestDto createAgentRequest)
    {
        Domain.Model.Agent agent = new Domain.Model.Agent
        {
            Name = createAgentRequest.AgentId,
            ApplicationTrigram = createAgentRequest.ApplicationTrigram,
            ComponentName = createAgentRequest.ComponentName,
            AgentType = createAgentRequest.AgentType,
        };
        
        return agent;
    }
    
    private async Task ProcessCommandAsync(Domain.Model.Agent agent, Command command, Action<Domain.Model.Agent, Command> commandHandler)
    {
       if (command == null)
           throw new ArgumentNullException(nameof(command));

       if (command.Name == CommandName.WhoAmI)
       {
           await ProcessWhoAmICommand(agent, command);
           return;
       }
       
       commandHandler(agent,command);
    }
    
    private async Task ProcessWhoAmICommand(Domain.Model.Agent agent,Command command)
    {
        var whoAmIResponse = new StandardWhoAmIResponse
        {
            AgentName = agent.Name,
            ApplicationTrigram = agent.ApplicationTrigram
        };
        
        var response = new Response
        {
            ApplicationTrigram = agent.ApplicationTrigram,
            FromAgent = agent.Name,
            FromCommand = command,
            Payload = JsonSerializer.Serialize(whoAmIResponse)
        };
        
        await _responseMessagePublisher.PublishAsync(agent, response);
    }
    
}