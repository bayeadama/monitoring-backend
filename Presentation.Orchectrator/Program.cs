using Application;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Presentation.Orchestrator;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

builder.Services.AddSignalR();
services.AddSingleton<StateMachineConfigBuilder>();
services.AddInfrastructureServices();
services.AddApplicationServices();

services.AddHostedService<Worker>();

var app = builder.Build();

app.MapHub<MonitoringHub>("/hubs/monitoring");

app.Run();