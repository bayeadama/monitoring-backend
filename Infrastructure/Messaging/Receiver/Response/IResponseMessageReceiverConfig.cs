namespace Infrastructure.Messaging.Receiver.Response;

public interface IResponseMessageReceiverConfig
{
    public string ResponseExchange { get; }
}