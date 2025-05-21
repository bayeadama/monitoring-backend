using Common.Listener;
using Common.Models;

namespace Common.Commander;

public interface ICommander : IListener
{
    Task SendCommandAsync(Command command, string destination);
}

