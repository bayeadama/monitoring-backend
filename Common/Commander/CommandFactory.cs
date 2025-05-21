using Common.Models;

namespace Common.Commander;

public static class CommandFactory
{
    public static Command CreateWhoAmiCommand()
    {
        return new Command
        {
            RequestId = Guid.NewGuid(),
            Name = CommandName.WhoAmI,
            CreatedAt = DateTime.UtcNow
        };
    }
    
    public static Command CreateMonitoringCommand()
    {
        return new Command
        {
            RequestId = Guid.NewGuid(),
            Name = CommandName.Monitoring,
            CreatedAt = DateTime.UtcNow
        };
    }
}