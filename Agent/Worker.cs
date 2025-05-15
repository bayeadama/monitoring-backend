using System.Text;
using Common;
using Common.Constants;
using Common.Models;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;

namespace Agent;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IChannelFactory _channelFactory;
    private readonly IQueueSetupFactory _queueSetupFactory;
    private const string FANOUT_EXCHANGE = "main.monitoring.fanout";
    private const string DIRECT_EXCHANGE = "main.monitoring.exchange";
    private const string DEAD_LETTERS_EXCHANGE = "dead-letters-exchange";
    private const string QUEUE_NAME = "queue.monitoring.msvgvapbpevx21p";
    private const string MAIN_EXCHANGE = "main.monitoring";
    private const string ADDRESS = "ampq://guest:guest@localhost:5603";
    private IAgentFactory  _agentFactory;

    public Worker(ILogger<Worker> logger, IChannelFactory channelFactory, IQueueSetupFactory queueSetupFactory, IAgentFactory agentFactory)
    {
        _logger = logger;
        _channelFactory = channelFactory;
        _queueSetupFactory = queueSetupFactory;
        _agentFactory = agentFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var agentConfig = BuildAgentConfig(); 
        var agent = await _agentFactory.Create(agentConfig);
        
        /*await using var channel = await _channelFactory.CreateChannelAsync(new Uri(ADDRESS));
        
        await SetupQueue(channel, "pg.pgs");
        await SetupQueue(channel, "pg.bbc");
        //await channel.AddConsumerAsync(queueDeclareResult.QueueName, MessageReceived);
       */
        agent.OnCommandReceived += async (sender, command) =>
        {
            _logger.LogInformation($"Command received: {command}");
            await HandleCommand(sender, command);
            
        };
        
        while (!stoppingToken.IsCancellationRequested)
        {
            /*await channel.PublishAsync($"hello there {DateTime.UtcNow}", 
                                        MAIN_EXCHANGE,
                                        "all");
                                        */
            
            await Task.Delay(15000, stoppingToken);
        }
    }

    async Task HandleCommand(IAgent agent, Command command)
    {
        switch (command.Name)
        {
            case CommandNames.Refresh:
            {
                var response = new Response(command.RequestId, agent.Id, command.Name, "all good");
                await agent.SendResponseAsync(response);
            }
                break;
        }
    }

    private AgentConfig BuildAgentConfig()
    {
        var agentConfig = new AgentConfig
        {
            AgentId = "pg.pgs",
            ResponseExchange = "main.monitoring.response",
            BroadcastExchange = FANOUT_EXCHANGE,
            DeadLettersExchange = DEAD_LETTERS_EXCHANGE,
            DirectExchange = DIRECT_EXCHANGE,
            ServerUri = new Uri(ADDRESS)
        };
        
        return agentConfig;
    }

    private async Task SetupQueue(IChannel channel, string queueName)
    {
        await BindToFanoutExchange(channel, queueName);
        await BindToDirectExchange(channel, queueName);
    }

    private async Task BindToFanoutExchange(IChannel channel, string queueName)
    {
        var queueSetup = _queueSetupFactory.CreateQueueSetupBoundToExchange(queueName,FANOUT_EXCHANGE);
        queueSetup.AddArgument(ArgumentNames.TimeToLive, 1000);
        queueSetup.AddArgument(ArgumentNames.DeadLetterExchange, DEAD_LETTERS_EXCHANGE);
        await channel.SetupQueueAsync(queueSetup);
    }
    
    private async Task BindToDirectExchange(IChannel channel, string queueName)
    {
        var queueSetup = _queueSetupFactory.CreateQueueSetupBoundToExchange(queueName,DIRECT_EXCHANGE,queueName);
        queueSetup.AddArgument(ArgumentNames.TimeToLive, 1000);
        queueSetup.AddArgument(ArgumentNames.DeadLetterExchange, DEAD_LETTERS_EXCHANGE);
        await channel.SetupQueueAsync(queueSetup);
    }

    private async Task MessageReceived(object sender, BasicDeliverEventArgs args)
    {
        byte[] body = args.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        _logger.LogInformation($"Received: {message}");
    }
    
    
}