using Application;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Presentation.Orchestrator;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "corsPolicy1",
        policy  =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

services.AddSingleton<StateMachineConfigBuilder>();
services.AddInfrastructureServices();
services.AddApplicationServices();

services.AddHostedService<Worker>();

var app = builder.Build();

app.UseCors();
app.MapHub<MonitoringHub>("/hubs/monitoring").RequireCors("corsPolicy1");

app.Run();