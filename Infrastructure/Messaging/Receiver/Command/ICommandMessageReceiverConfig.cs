namespace Infrastructure.Messaging.Receiver.Command;

public interface ICommandMessageReceiverConfig
{
    string CommanderExchange { get; }
    string DeadLettersExchange { get; }
}