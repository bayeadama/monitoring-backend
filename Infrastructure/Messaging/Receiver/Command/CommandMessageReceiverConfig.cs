namespace Infrastructure.Messaging.Receiver.Command;

public class CommandMessageReceiverConfig : ICommandMessageReceiverConfig
{
    public string CommanderExchange
    {
        get
        {
            return "commander.main.exchange";
        }
    }

    public string DeadLettersExchange
    {
        get
        {
            return "dead-letters.pbp.exchange";
        }
    }
}