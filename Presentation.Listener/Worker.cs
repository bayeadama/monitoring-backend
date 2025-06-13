using Application.Services.Listener;

namespace Presentation.Listener;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IListenerApplicationService _listenerApplicationService;

    public Worker(ILogger<Worker> logger, IListenerApplicationService listenerApplicationService)
    {
        _logger = logger;
        _listenerApplicationService = listenerApplicationService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Domain.Model.Listener listener = await _listenerApplicationService.InitializeListenerAsync("Listener1");
        _logger.LogInformation("Listener initialized: {ListenerName}", listener.Name);
        
        await _listenerApplicationService.RegisterResponseHandlerAsync(listener, ResponseHandler);
        
        await RunInfinitely(stoppingToken);
    }
    
    private void ResponseHandler(Domain.Model.Listener listener, Domain.Model.ValueObjects.Response response)
    {
        if (listener == null) throw new ArgumentNullException(nameof(listener));
        if (response == null) throw new ArgumentNullException(nameof(response));
        
        
        _logger.LogInformation("Response received: {Response}", response.Payload);
    }
    
    private static async Task RunInfinitely(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}