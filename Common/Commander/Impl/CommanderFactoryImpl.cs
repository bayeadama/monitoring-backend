using RabbitMQ.Client;

namespace Common.Commander;

public class CommanderFactoryImpl: ICommanderFactory
{
    private readonly IQueueSetupFactory _queueSetupFactory;

    public CommanderFactoryImpl(IQueueSetupFactory queueSetupFactory)
    {
        _queueSetupFactory = queueSetupFactory ?? throw new ArgumentNullException(nameof(queueSetupFactory));;
    }

    public async Task<ICommander> Create(CommanderConfig commanderConfig)
    {
        if(commanderConfig == null)
            throw new ArgumentNullException(nameof(commanderConfig));
        
        IChannel channel = await commanderConfig.ServerUri.CreateChannelAsync();
        ICommander commander = new DefaultCommander(commanderConfig, channel, _queueSetupFactory);
        
        return commander;
    }
}