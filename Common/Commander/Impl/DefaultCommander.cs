using System.Text.Json;
using Common.Listener.Impl;
using Common.Models;
using RabbitMQ.Client;

namespace Common.Commander;

public class DefaultCommander : DefaultListener, ICommander
{
    private readonly CommanderConfig _config;
    private readonly IChannel _channel;

    public DefaultCommander(CommanderConfig config, IChannel channel, IQueueSetupFactory queueSetupFactory) :
        base(config, channel, queueSetupFactory)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _channel = channel  ?? throw new ArgumentNullException(nameof(channel));
    }
    
    public async Task SendCommandAsync(Command command, string destination)
    {
        string commanderExchange =  _config.CommanderExchange;
        string commandSerialized = JsonSerializer.Serialize(command);
        await _channel.PublishAsync(commandSerialized, commanderExchange, destination);
    }
}