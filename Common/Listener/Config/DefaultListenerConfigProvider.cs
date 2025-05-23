using Microsoft.Extensions.Configuration;

namespace Common.Listener.Config;

public class DefaultListenerConfigProvider : BaseConfigProvider, IListenerConfigProvider
{
    private readonly IConfiguration  _configuration;
    private const string DefaultConfigSection = "Monitoring.Listener";
    private const string ServerUriKey = "ServerUri";
    private const string ResponseExchangeKey = "ResponseExchange";

    public DefaultListenerConfigProvider(IConfiguration configuration, string configurationSection= DefaultConfigSection):
        base(configurationSection)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public ListenerConfig GetConfig()
    {
        return new ListenerConfig
        {
            ServerUri = new Uri(_configuration[GetConfigKey(ServerUriKey)]),
            ResponseExchange = _configuration[GetConfigKey(ResponseExchangeKey)]
        };
    }
    
}