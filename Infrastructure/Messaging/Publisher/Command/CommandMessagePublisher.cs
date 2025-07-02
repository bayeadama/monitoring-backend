using System.Text.Json;
using Application.Interfaces;
using Domain.Model;
using Domain.Model.ValueObjects;
using Infrastructure.Extensions;
using RabbitMQ.Client;

namespace Infrastructure.Messaging.Publisher;

public class CommandMessagePublisher : IMessagePublisher<Domain.Model.Commander, Command>
{
    private readonly ICommandMessagePublisherConfig _config;
    private readonly IChannel _channel;

    public CommandMessagePublisher(ICommandMessagePublisherConfig config, IChannel channel)
    {
        _config = config;
        _channel = channel;
    }

    public async Task PublishAsync(Commander publisher, Command command, string destination = null)
    {
        string commandSerialized = JsonSerializer.Serialize(command);
        await _channel.PublishAsync(commandSerialized, _config.CommandExchange, destination);
    }
}