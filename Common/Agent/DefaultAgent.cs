using System.Text;
using System.Text.Json;
using Common.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Common;

public class DefaultAgent : IAgent
{
    private readonly AgentConfig _agentInitializerConfig;
    private readonly IChannel _channel;
    
    public DefaultAgent(AgentConfig agentInitializerConfig, IChannel channel)
    {
        _agentInitializerConfig = agentInitializerConfig ?? throw new ArgumentNullException(nameof(agentInitializerConfig));
        _channel = channel ?? throw new ArgumentNullException(nameof(channel));
        
        Id = _agentInitializerConfig.AgentId;
        _ = SetupConsumers();
    }

    private async Task SetupConsumers()
    {
        string queueName = $"agent.{_agentInitializerConfig.AgentId}";
        await _channel.AddConsumerAsync(queueName, MessageReceived);
    }

    public string Id { get; set; }
    public event OnCommandReceivedDelegate? OnCommandReceived;
    public async Task SendResponseAsync(Response response)
    {
        string responseExchange =  _agentInitializerConfig.ResponseExchange;
        string routingKey = _agentInitializerConfig.AgentId;
        string responseSerialized = JsonSerializer.Serialize(response);
        await _channel.PublishAsync(responseSerialized, responseExchange, routingKey);
    }
    
    private Task MessageReceived(object sender, BasicDeliverEventArgs args)
    {
        byte[] body = args.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var command = JsonSerializer.Deserialize<Command>(message);

        if (command != null) 
            OnCommandReceived?.Invoke(this, command);
        
        return Task.CompletedTask;
    }
}