using RabbitMQ.Client;

namespace Common;

public interface IChannelFactory
{
    Task<IChannel> CreateChannelAsync(Uri uri);
}