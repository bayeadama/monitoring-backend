using System.Text.Json;
using Common.Listener;
using Common.Listener.Config;
using Common.Models;

namespace Listener;

public class ListenerWorker : BackgroundService
{
    private readonly ILogger<ListenerWorker> _logger;
    private IListenerFactory  _listenerFactory;
    private const string RESPONSE_EXCHANGE = "commander.response.exchange";
    private const string ADDRESS = "ampq://guest:guest@localhost:5603/monitoring";
    
    public ListenerWorker(ILogger<ListenerWorker> logger, IListenerFactory listenerFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _listenerFactory = listenerFactory  ?? throw new ArgumentNullException(nameof(listenerFactory));;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ListenerConfig config = BuildListenerConfig();
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

    private ListenerConfig BuildListenerConfig()
    {
        var agentConfig = new ListenerConfig
        {
            ResponseExchange = RESPONSE_EXCHANGE,
            ServerUri = new Uri(ADDRESS)
        };
        
        return agentConfig;
    }
}