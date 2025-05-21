namespace Common;

public class AgentConfig
{
    public string ApplicationTrigram { get; set; }
    public string ComponentName { get; set; }
    public string? AgentId { get; set; }
    public Uri ServerUri { get; set; }
    
    public string CommanderExchange { get; set; }
    public string ResponseExchange { get; set; }
    public string DeadLettersExchange { get; set; }
}