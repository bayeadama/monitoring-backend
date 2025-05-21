using System.Text;
using System.Text.Json;
using Common.Commander;
using Common.Listener.Config;
using Common.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Common.Listener.Impl;

public class DefaultListener : IListener
{
    private readonly ListenerConfig _config;
    private readonly IChannel _channel;
    private readonly IQueueSetupFactory _queueSetupFactory;

    public DefaultListener(ListenerConfig config, IChannel channel, IQueueSetupFactory queueSetupFactory)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _channel = channel  ?? throw new ArgumentNullException(nameof(channel));
        _queueSetupFactory = queueSetupFactory ?? throw new ArgumentNullException(nameof(queueSetupFactory));

        _ = SetupConsumers();
    }

    private async Task SetupConsumers()
    {
        var queueSetup = _queueSetupFactory.CreateArbitraryQueueSetupBoundToExchange(_config.ResponseExchange);
        var queueDeclareOk = await _channel.SetupQueueAsync(queueSetup);
        await _channel.AddConsumerAsync(queueDeclareOk.QueueName, MessageReceived);
    }
    
    private Task MessageReceived(object sender, BasicDeliverEventArgs args)
    {
        byte[] body = args.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var response = JsonSerializer.Deserialize<Response>(message);

        if (response != null) 
            OnResponseReceived?.Invoke(response);
        
        return Task.CompletedTask;
    }

    public event OnResponseReceivedDelegate? OnResponseReceived;
}