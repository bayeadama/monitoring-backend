using Domain.Base;

namespace Domain.Model;

public class Agent : BaseEntity
{
    /// <summary>
    /// Agent's name
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Related application's trigram
    /// </summary>
    public string ApplicationTrigram { get; set; }

    /// <summary>
    /// Monitored component's name
    /// </summary>
    public string ComponentName { get; set; }

    /// <summary>
    /// Agent's type
    /// </summary>
    public AgentType AgentType { get; set; }
    
}