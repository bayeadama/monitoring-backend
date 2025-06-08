using System.Text;
using System.Text.Json;
using Application.Dto.Requests;
using Application.Interfaces;
using Domain.Model;
using Domain.Model.ValueObjects;

namespace Application.Services;

public class AgentApplicationService
{
    private readonly IMessagePublisher<Agent, Response> _responseMessagePublisher;
    private readonly IMessageReceiver<Agent, Command> _commandReceiver;

    public AgentApplicationService(IMessagePublisher<Agent, Response> responseMessagePublisher,
        IMessageReceiver<Agent, Command> commandReceiver)
    {
        _responseMessagePublisher = responseMessagePublisher ??
                                    throw new ArgumentNullException(nameof(responseMessagePublisher));
        _commandReceiver = commandReceiver ?? throw new ArgumentNullException(nameof(commandReceiver));
    }
    
    public Task<Agent> InitializeAgentAsync(CreateAgentRequestDto createAgentRequest)
    {
        if (createAgentRequest == null)
        {
            throw new ArgumentNullException(nameof(createAgentRequest));
        }

        Agent agent = InitAgent(createAgentRequest);
       
        return Task.FromResult(agent);
    }

    public async Task RegisterCommandHandlerAsync(Agent agent, Action<Agent,Command> commandHandler)
    {
        if(agent == null)
            throw new ArgumentNullException(nameof(agent));
        
        if (commandHandler == null)
            throw new ArgumentNullException(nameof(commandHandler));
        
        await _commandReceiver.RegisterHandlerAsync(agent, (command) => ProcessCommandAsync(agent, command, commandHandler));
    }

    private Agent InitAgent(CreateAgentRequestDto createAgentRequest)
    {
        Agent agent = new Agent
        {
            Name = createAgentRequest.AgentId,
            ApplicationTrigram = createAgentRequest.ApplicationTrigram,
            ComponentName = createAgentRequest.ComponentName,
            AgentType = createAgentRequest.AgentType,
        };
        
        return agent;
    }
    
    private async Task ProcessCommandAsync(Agent agent, Command command, Action<Agent, Command> commandHandler)
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
    
    private async Task ProcessWhoAmICommand(Agent agent,Command command)
    {
        var whoAmIResponse = new StandardWhoAmIResponse
        {
            AgentName = agent.Name,
            ApplicationTrigram = agent.ApplicationTrigram
        };
        
        var response = new Response
        {
            FromCommand = command,
            Payload = JsonSerializer.Serialize(whoAmIResponse)
        };
        
        await _responseMessagePublisher.PublishAsync(agent, response);
    }
    
}