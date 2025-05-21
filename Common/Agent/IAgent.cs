using Common.Models;

namespace Common;

public interface IAgent
{
    string Id { get; set; }
    public string ApplicationTrigram { get; set; }

    public string ComponentName { get; set; }

    event OnCommandReceivedDelegate OnCommandReceived;
    
    Task SendResponseAsync(Response response);
}

public delegate void OnCommandReceivedDelegate(IAgent agent, Command command);