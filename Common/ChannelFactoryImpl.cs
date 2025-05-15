using RabbitMQ.Client;

namespace Common;

public class ChannelFactoryImpl: IChannelFactory
{
    public async Task<IChannel> CreateChannelAsync(Uri uri)
    {
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
        var channel = await connection.CreateChannelAsync();
        return channel;
    }
}