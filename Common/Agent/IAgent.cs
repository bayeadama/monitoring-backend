using Common.Models;

namespace Common;

public interface IAgent
{
    /// <summary>
    /// Agent's identifier
    /// </summary>
    string Id { get; set; }
    
    /// <summary>
    /// Related application's trigram
    /// </summary>
    public string ApplicationTrigram { get; set; }

    /// <summary>
    /// Monitored component's name
    /// </summary>
    public string ComponentName { get; set; }
    
    /// <summary>
    /// Sends analysis' response
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    Task SendResponseAsync(Response response);
    

    /// <summary>
    /// Event
    /// </summary>
    event OnCommandReceivedDelegate OnCommandReceived;
    
}

public delegate void OnCommandReceivedDelegate(IAgent agent, Command command);