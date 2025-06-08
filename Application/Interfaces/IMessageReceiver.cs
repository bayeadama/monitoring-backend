namespace Application.Interfaces;

public interface IMessageReceiver<TReceiver, TMessage> where TMessage : class 
                                                       where TReceiver : class
{
    public Task RegisterHandlerAsync(TReceiver receiver, Func<TMessage, Task> handler);
}