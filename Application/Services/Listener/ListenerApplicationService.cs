using Application.Interfaces;
using Domain.Model.ValueObjects;

namespace Application.Services.Listener;

public class ListenerApplicationService : IListenerApplicationService
{
    private readonly IMessageReceiver<Domain.Model.Listener, Response> _responseReceiver;

    public ListenerApplicationService(IMessageReceiver<Domain.Model.Listener, Response> responseReceiver)
    {
        _responseReceiver = responseReceiver;
    }

    /// <inheritdoc />
    public Task<Domain.Model.Listener> InitializeListenerAsync(string listenerName)
    {
        var listener = new Domain.Model.Listener { Name = listenerName };

        return Task.FromResult(listener);
    }

    /// <inheritdoc />
    public async Task RegisterResponseHandlerAsync(Domain.Model.Listener listener,
        Action<Domain.Model.Listener, Response> responseHandler)
    {
        if (listener == null) throw new ArgumentNullException(nameof(listener));
        if (responseHandler == null) throw new ArgumentNullException(nameof(responseHandler));

        await _responseReceiver.RegisterHandlerAsync(listener,
            (response) => ProcessResponseAsync(listener, response, responseHandler));
    }

    private async Task ProcessResponseAsync(Domain.Model.Listener listener, Response response,
        Action<Domain.Model.Listener, Response> responseHandler)
    {
        if (listener == null) throw new ArgumentNullException(nameof(listener));
        if (response == null) throw new ArgumentNullException(nameof(response));
        if (responseHandler == null) throw new ArgumentNullException(nameof(responseHandler));

        responseHandler(listener, response);
    }
}