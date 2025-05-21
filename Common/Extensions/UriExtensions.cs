using RabbitMQ.Client;

namespace Common;

public static class UriExtensions
{
    private static readonly Dictionary<string,IConnection> ConnectionsCache = new();

    public static async Task<IChannel> CreateChannelAsync(this Uri uri)
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