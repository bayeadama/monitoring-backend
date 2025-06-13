using System.Text.Json;
using Application.Interfaces;
using Domain.Model;
using Domain.Model.ValueObjects;
using Infrastructure.Extensions;
using RabbitMQ.Client;

namespace Infrastructure.Messaging.Publisher;

public class ResponseMessagePublisher : IMessagePublisher<Agent, Response>
{
    private readonly IResponseMessagePublisherConfig _config;
    private readonly IChannel _channel;

    public ResponseMessagePublisher(IResponseMessagePublisherConfig config, IChannel channel)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _channel = channel ?? throw new ArgumentNullException(nameof(channel));
    }

    public async Task PublishAsync(Agent agent, Response message)
    {
        string responseExchange = _config.ResponseExchange;
        string? routingKey = agent.Name;
        string responseSerialized = JsonSerializer.Serialize(message);
        await _channel.PublishAsync(responseSerialized, responseExchange, routingKey);
    }
}