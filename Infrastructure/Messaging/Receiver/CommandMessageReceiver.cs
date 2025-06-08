using System.Text;
using System.Text.Json;
using Application.Interfaces;
using Domain.Model;
using Domain.Model.ValueObjects;
using Infrastructure.Constants;
using Infrastructure.Extensions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Infrastructure.Messaging.Receiver;

public class CommandMessageReceiver : IMessageReceiver<Agent, Command>
{
    private const int TimeToLiveMilliseconds = 1000;

    private readonly CommandMessageReceiverConfig _config;
    private readonly IChannel _channel;
    private readonly IQueueSetupFactory _queueSetupFactory;

    public CommandMessageReceiver(CommandMessageReceiverConfig config, IChannel channel,
        IQueueSetupFactory queueSetupFactory)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _channel = channel ?? throw new ArgumentNullException(nameof(channel));
        _queueSetupFactory = queueSetupFactory ?? throw new ArgumentNullException(nameof(queueSetupFactory));
    }

    public async Task RegisterHandlerAsync(Agent agent, Func<Command, Task> handler)
    {
        string queueName = $"agent.{agent.Name}";
        await BindToCommanderExchangeAsync(_channel, queueName, agent);
        await _channel.AddConsumerAsync(queueName, (sender, args) => MessageReceived(sender, args, handler));
    }

    private async Task MessageReceived(object sender, BasicDeliverEventArgs args, Func<Command, Task> handler)
    {
        byte[] body = args.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var command = JsonSerializer.Deserialize<Command>(message);

        if (command == null) return;

        await handler(command);
    }

    private async Task BindToCommanderExchangeAsync(IChannel channel, string queueName, Agent agent)
    {
        var routingKeys = new List<string>
        {
            "all",
            $"{agent.ApplicationTrigram}.all",
            queueName
        };

        var queueSetup = _queueSetupFactory.CreateQueueSetupBoundToExchange(
            queueName,
            _config.CommanderExchange,
            routingKeys);
        queueSetup.AddArgument(ArgumentNames.TimeToLive, TimeToLiveMilliseconds);
        queueSetup.AddArgument(ArgumentNames.DeadLetterExchange, _config.DeadLettersExchange);
        await channel.SetupQueueAsync(queueSetup);
    }
}