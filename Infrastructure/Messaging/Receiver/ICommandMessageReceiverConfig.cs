namespace Infrastructure.Messaging.Receiver;

public interface ICommandMessageReceiverConfig
{
    string CommanderExchange { get; }
    string DeadLettersExchange { get; }
}