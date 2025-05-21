using RabbitMQ.Client;

namespace Common.Agent;

public class AgentFactoryImpl : IAgentFactory
{
    private const int TimeToLiveMilliseconds = 1000;
    private readonly IQueueSetupFactory _queueSetupFactory;

    public AgentFactoryImpl(IQueueSetupFactory queueSetupFactory)
    {
        _queueSetupFactory = queueSetupFactory ?? throw new ArgumentNullException(nameof(queueSetupFactory));;
    }


    public async Task<IAgent> Create(AgentConfig agentConfig)
    {
        if(agentConfig == null)
            throw new ArgumentNullException(nameof(agentConfig));
        
        IChannel channel = await agentConfig.ServerUri.CreateChannelAsync();
        await SetupQueueAsync(agentConfig, channel);
        IAgent agent = new DefaultAgent(channel, agentConfig);
        
        return agent;
    }
    
    
    private async Task SetupQueueAsync(AgentConfig agentConfig, IChannel channel)
    {
        string queueName = $"agent.{agentConfig.AgentId}";
        await BindToCommanderExchangeAsync(channel, queueName, agentConfig);
    }


    private async Task BindToCommanderExchangeAsync(IChannel channel, string queueName, AgentConfig agentConfig)
    {
        var routingKeys = new List<string>
        {
            "all",
            $"{agentConfig.ApplicationTrigram}.all",
            queueName
        };
        
        var queueSetup = _queueSetupFactory.CreateQueueSetupBoundToExchange(
            queueName,
            agentConfig.CommanderExchange,
            routingKeys);
        queueSetup.AddArgument(ArgumentNames.TimeToLive, TimeToLiveMilliseconds);
        queueSetup.AddArgument(ArgumentNames.DeadLetterExchange, agentConfig.DeadLettersExchange);
        await channel.SetupQueueAsync(queueSetup);
    }


   
}