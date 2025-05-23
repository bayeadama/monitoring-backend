using Common.Listener.Config;
using Microsoft.Extensions.Configuration;

namespace Common.Commander;

public class DefaultCommanderConfigProvider: DefaultListenerConfigProvider, ICommanderConfigProvider
{
    private readonly IConfiguration  _configuration;
    private const string DefaultConfigSection = "Monitoring.Commander";
    private const string CommanderExchangeKey = "CommanderExchange";



    public DefaultCommanderConfigProvider(IConfiguration configuration, string configurationSection= DefaultConfigSection):
    base(configuration, configurationSection)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public CommanderConfig GetConfig()
    {
        ListenerConfig baseConfig = base.GetConfig();
        var commanderConfig = new CommanderConfig(baseConfig)
        {
            CommanderExchange = _configuration[GetConfigKey(CommanderExchangeKey)]
        };

        return commanderConfig;
    }
}