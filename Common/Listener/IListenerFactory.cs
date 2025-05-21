using Common.Commander;
using Common.Listener.Config;

namespace Common.Listener;

public interface IListenerFactory
{
    Task<IListener> Create(ListenerConfig listenerConfig);
}