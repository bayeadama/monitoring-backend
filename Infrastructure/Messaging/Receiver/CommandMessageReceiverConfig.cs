namespace Infrastructure.Messaging.Receiver;

public class CommandMessageReceiverConfig
{
    public string CommanderExchange { get; set; }
    public string DeadLettersExchange { get; set; }
}