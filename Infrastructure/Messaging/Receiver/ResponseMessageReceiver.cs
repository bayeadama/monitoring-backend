using System.Text;
using System.Text.Json;
using Application.Interfaces;
using Domain.Model;
using Domain.Model.ValueObjects;
using Infrastructure.Extensions;
using RabbitMQ.Client;

namespace Infrastructure.Messaging.Receiver;

public class ResponseMessageReceiver : IMessageReceiver<Listener, Response>
{
    private readonly IChannel _channel;
    private readonly IQueueSetupFactory _queueSetupFactory;
    private readonly IResponseMessageReceiverConfig _config;

    public ResponseMessageReceiver(IChannel channel, IQueueSetupFactory queueSetupFactory, IResponseMessageReceiverConfig config)
    {
        _channel = channel;
        _queueSetupFactory = queueSetupFactory;
        _config = config;
    }

    public async Task RegisterHandlerAsync(Listener receiver, Func<Response, Task> handler)
    {
        string createdQueueName = await BindToResponseExchangeAsync();
        await SetupResponseHandlerAsync(createdQueueName, handler);
    }

    private async Task<string> BindToResponseExchangeAsync()
    {
        var queueSetup = _queueSetupFactory.CreateArbitraryQueueSetupBoundToExchange(_config.ResponseExchange);
        var queueDeclareOk = await _channel.SetupQueueAsync(queueSetup);
        
        return queueDeclareOk.QueueName;
    }
    
    private async Task SetupResponseHandlerAsync(string queueName, Func<Response, Task> handler)
    {
        await _channel.AddConsumerAsync(queueName, async (sender, args) =>
        {
            byte[] body = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var response = JsonSerializer.Deserialize<Response>(message);

            if (response != null)
            {
                await handler(response);
            }
        });
    }
    
}