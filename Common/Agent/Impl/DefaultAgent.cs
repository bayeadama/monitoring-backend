using System.Text;
using System.Text.Json;
using Common.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Common.Agent;

public class DefaultAgent : IAgent
{
    private readonly AgentConfig _agentConfig;
    private readonly IChannel _channel;
    
    public DefaultAgent(IChannel channel, AgentConfig agentConfig)
    {
        _channel = channel ?? throw new ArgumentNullException(nameof(channel));
        _agentConfig = agentConfig ?? throw new ArgumentNullException(nameof(agentConfig));

        Id = _agentConfig.AgentId;
        ApplicationTrigram = _agentConfig.ApplicationTrigram;
        ComponentName = _agentConfig.ComponentName;
        _ = SetupConsumers();
    }

    private async Task SetupConsumers()
    {
        string queueName = $"agent.{_agentConfig.AgentId}";
        await _channel.AddConsumerAsync(queueName, MessageReceived);
    }

    public string? Id { get; set; }
    public string ApplicationTrigram { get; set; }
    public string ComponentName { get; set; }
    public event OnCommandReceivedDelegate? OnCommandReceived;
    public async Task SendResponseAsync(Response response)
    {
        string responseExchange =  _agentConfig.ResponseExchange;
        string? routingKey = _agentConfig.AgentId;
        string responseSerialized = JsonSerializer.Serialize(response);
        await _channel.PublishAsync(responseSerialized, responseExchange, routingKey);
    }
    
    private async Task MessageReceived(object sender, BasicDeliverEventArgs args)
    {
        byte[] body = args.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var command = JsonSerializer.Deserialize<Command>(message);

        if (command == null) return ;

        if (command.Name == CommandName.WhoAmI)
            await ProcessWhoAmICommand(command);
        
        OnCommandReceived?.Invoke(this, command);
    }

    private async Task ProcessWhoAmICommand(Command command)
    {
        var whoAmIResponse = new StandardWhoAmIResponse
        {
            AgentId = _agentConfig.AgentId,
            ApplicationTrigram = _agentConfig.ApplicationTrigram
        };
        
        var response = new Response
        {
            FromCommand = command,
            Payload = JsonSerializer.Serialize(whoAmIResponse)
        };
        await SendResponseAsync(response);
    }
}