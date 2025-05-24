using System.Text.Json;
using Common.Listener;
using Common.Listener.Config;
using Common.Models;

namespace Listener;

public class ListenerWorker : BackgroundService
{
    private readonly ILogger<ListenerWorker> _logger;
    private readonly IListenerFactory  _listenerFactory;
    private readonly IListenerConfigProvider  _listenerConfigProvider;
    
    public ListenerWorker(ILogger<ListenerWorker> logger, IListenerFactory listenerFactory, IListenerConfigProvider listenerConfigProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _listenerFactory = listenerFactory  ?? throw new ArgumentNullException(nameof(listenerFactory));
        _listenerConfigProvider = listenerConfigProvider ?? throw new ArgumentNullException(nameof(listenerConfigProvider));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ListenerConfig config = _listenerConfigProvider.GetConfig();
        IListener listener = await _listenerFactory.Create(config);
        listener.OnResponseReceived += OnResponseReceived;
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    private void OnResponseReceived(Response response)
    {
        string json = JsonSerializer.Serialize(response);
        _logger.LogInformation($"Received response (in listener): {json}");

    }

}