using Microsoft.Extensions.Configuration;

namespace Common;

public class DefaultAgentConfigProvider : IAgentConfigProvider
{
    private readonly IConfiguration  _configuration;
    private const string ApplicationTrigramKey = "Monitoring.Agent:ApplicationTrigram";
    private const string ComponentNameKey = "Monitoring.Agent:ComponentName";
    private const string AgentIdKey = "Monitoring.Agent:AgentId";
    private const string ServerUriKey = "Monitoring.Agent:ServerUri";
    private const string CommanderExchangeKey = "Monitoring.Agent:CommanderExchange";
    private const string ResponseExchangeKey = "Monitoring.Agent:ResponseExchange";
    private const string DeadLetterExchangeKey = "Monitoring.Agent:DeadLetterExchange";
    
    
    public DefaultAgentConfigProvider(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }
    public AgentConfig GetConfig()
    {
        return new AgentConfig
        {
            ApplicationTrigram = _configuration[ApplicationTrigramKey],
            ComponentName = _configuration[ComponentNameKey],
            ServerUri = new Uri(_configuration[ServerUriKey]),
            AgentId = _configuration[AgentIdKey],
            CommanderExchange = _configuration[CommanderExchangeKey],
            DeadLettersExchange = _configuration[DeadLetterExchangeKey],
            ResponseExchange = _configuration[ResponseExchangeKey]
        };
    }
}