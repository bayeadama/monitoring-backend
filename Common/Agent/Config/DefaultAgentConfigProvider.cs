using Microsoft.Extensions.Configuration;

namespace Common;

public class DefaultAgentConfigProvider : BaseConfigProvider, IAgentConfigProvider
{
    private readonly IConfiguration  _configuration;
    private const string DefaultConfigSection = "Monitoring.Agent";
    private const string ApplicationTrigramKey = "ApplicationTrigram";
    private const string ComponentNameKey = "ComponentName";
    private const string AgentIdKey = "AgentId";
    private const string ServerUriKey = "ServerUri";
    private const string CommanderExchangeKey = "CommanderExchange";
    private const string ResponseExchangeKey = "ResponseExchange";
    private const string DeadLetterExchangeKey = "DeadLetterExchange";
    
    
    public DefaultAgentConfigProvider(IConfiguration configuration, string configurationSection = DefaultConfigSection):
        base(configurationSection)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }
    public AgentConfig GetConfig()
    {
        return new AgentConfig
        {
            ApplicationTrigram = _configuration[GetConfigKey(ApplicationTrigramKey)],
            ComponentName = _configuration[GetConfigKey(ComponentNameKey)],
            ServerUri = new Uri(_configuration[GetConfigKey(ServerUriKey)]),
            AgentId = _configuration[GetConfigKey(AgentIdKey)],
            CommanderExchange = _configuration[GetConfigKey(CommanderExchangeKey)],
            DeadLettersExchange = _configuration[GetConfigKey(DeadLetterExchangeKey)],
            ResponseExchange = _configuration[GetConfigKey(ResponseExchangeKey)]
        };
    }
}