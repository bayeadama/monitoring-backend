namespace Infrastructure.Messaging.Receiver;

public class ResponseMessageReceiverConfig: IResponseMessageReceiverConfig
{
    public string ResponseExchange => "commander.response.exchange";
}