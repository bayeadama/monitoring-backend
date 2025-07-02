using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;

namespace Admin;

public class Worker : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
        HubConnection connection= new HubConnectionBuilder()
            .WithUrl("http://localhost:5000/hubs/monitoring")
            .Build();

        await connection.StartAsync();

        connection.On<string, string>("ReceiveStatusTransition", (fromStatus, toStatus) =>
        {
            Console.WriteLine($"Received status transition from {fromStatus} to {toStatus}");
        });
        
        while (!stoppingToken.IsCancellationRequested)
        {
            
            await Task.Delay(1000, stoppingToken); // Wait for 1 second before the next iteration
        }
    }
}