using RabbitMQ.Client;

namespace Common;

public class AgentFactoryImpl : IAgentFactory
{
    private const int TimeToLiveMilliseconds = 1000;
    private static readonly Dictionary<string,IConnection> ConnectionsCache = new();
    private readonly IQueueSetupFactory _queueSetupFactory;

    public AgentFactoryImpl(IQueueSetupFactory queueSetupFactory)
    {
        _queueSetupFactory = queueSetupFactory ?? throw new ArgumentNullException(nameof(queueSetupFactory));;
    }


    public async Task<IAgent> Create(AgentConfig agentInitializerConfig)
    {
        if(agentInitializerConfig == null)
            throw new ArgumentNullException(nameof(agentInitializerConfig));
        
        IChannel channel = await CreateChannelAsync(agentInitializerConfig.ServerUri);
        await SetupQueueAsync(agentInitializerConfig, channel);
        IAgent agent = new DefaultAgent(agentInitializerConfig, channel);
        
        return agent;
    }
    
    
    private async Task SetupQueueAsync(AgentConfig agentInitializerConfig, IChannel channel)
    {
        string queueName = $"agent.{agentInitializerConfig.AgentId}";
        await BindToCommanderExchangeAsync(channel, queueName, agentInitializerConfig);
    }


    private async Task BindToCommanderExchangeAsync(IChannel channel, string queueName, AgentConfig agentInitializerConfig)
    {
        var queueSetup = _queueSetupFactory.CreateQueueSetupBoundToExchange(queueName,agentInitializerConfig.CommanderExchange,queueName);
        queueSetup.AddArgument(ArgumentNames.TimeToLive, TimeToLiveMilliseconds);
        queueSetup.AddArgument(ArgumentNames.DeadLetterExchange, agentInitializerConfig.DeadLettersExchange);
        await channel.SetupQueueAsync(queueSetup);
    }


    private async Task<IChannel> CreateChannelAsync(Uri uri)
    {
        var connection = await CreateConnectionAsync(uri);
        var channel = await connection.CreateChannelAsync();
        return channel;
    }

    private static async Task<IConnection?> CreateConnectionAsync(Uri uri)
    {
        //Gets connection from cache
        if (TryGetCachedConnection(uri, out var cachedConnection)) 
            return cachedConnection;
        
        string[]? userSplits = uri.UserInfo?.Split(':', 2);
        string? user = null;
        string? password = null;
        if (userSplits != null && userSplits.Length == 2)
        {
            user = Uri.UnescapeDataString(userSplits[0]);
            password = Uri.UnescapeDataString(userSplits[1]);
        }

        var factory =new ConnectionFactory
        {
            HostName = uri.Host,
            Port = uri.IsDefaultPort ? 5672 : uri.Port,
            UserName = user,
            Password = password,
            VirtualHost = uri.AbsolutePath == "/" ? "/" : uri.AbsolutePath.Remove(0, 1),
            AutomaticRecoveryEnabled = true
        };
        
        var connection = await factory.CreateConnectionAsync();
        
        ConnectionsCache.Add(uri.AbsolutePath, connection);
        
        return connection;
    }

    private static bool TryGetCachedConnection(Uri uri, out IConnection? cachedConnection)
    {
        if(ConnectionsCache.TryGetValue(uri.AbsolutePath, out cachedConnection))
            return true;
        return false;
    }
}