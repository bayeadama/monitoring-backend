namespace Infrastructure.Messaging.Receiver.Response;

public class ResponseMessageReceiverConfig: IResponseMessageReceiverConfig
{
    public string ResponseExchange => "commander.response.exchange";
}