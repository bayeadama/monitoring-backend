using Common.Commander;
using Common.Listener.Config;
using RabbitMQ.Client;

namespace Common.Listener.Impl;

public class ListenerFactoryImpl : IListenerFactory
{
    private readonly IQueueSetupFactory _queueSetupFactory;

    public ListenerFactoryImpl(IQueueSetupFactory queueSetupFactory)
    {
        _queueSetupFactory = queueSetupFactory ?? throw new ArgumentNullException(nameof(queueSetupFactory));;
    }

    public async Task<IListener> Create(ListenerConfig listenerConfig)
    {
        if(listenerConfig == null)
            throw new ArgumentNullException(nameof(listenerConfig));
        
        IChannel channel = await listenerConfig.ServerUri.CreateChannelAsync();
        IListener listener = new DefaultListener(listenerConfig, channel, _queueSetupFactory);
        
        return listener;
    }
}