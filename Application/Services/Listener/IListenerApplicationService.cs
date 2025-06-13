namespace Application.Services.Listener;

public interface IListenerApplicationService
{
    /// <summary>
    /// Initializes a listener
    /// </summary>
    /// <param name="listenerName"></param>
    /// <returns></returns>
    Task<Domain.Model.Listener> InitializeListenerAsync(string listenerName);

    /// <summary>
    /// Registers a response's handler for the specified listener.
    /// </summary>
    /// <param name="listener"></param>
    /// <param name="responseHandler"></param>
    /// <returns></returns>
    Task RegisterResponseHandlerAsync(Domain.Model.Listener listener,
        Action<Domain.Model.Listener, Domain.Model.ValueObjects.Response> responseHandler);
}