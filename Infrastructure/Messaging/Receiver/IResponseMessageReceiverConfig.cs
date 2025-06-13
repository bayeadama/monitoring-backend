namespace Infrastructure.Messaging.Receiver;

public interface IResponseMessageReceiverConfig
{
    public string ResponseExchange { get; }
}