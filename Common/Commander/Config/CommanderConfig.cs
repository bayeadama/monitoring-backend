using Common.Listener.Config;

namespace Common.Commander;

public class CommanderConfig : ListenerConfig
{
    public CommanderConfig()
    {
        
    }

    public CommanderConfig(ListenerConfig  config)
    {
        this.ResponseExchange = config?.ResponseExchange;
        this.ServerUri = config?.ServerUri;
    }
    public string CommanderExchange { get; set; }
}