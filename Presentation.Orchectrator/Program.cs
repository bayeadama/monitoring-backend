using System.Text.Json;
using Application;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Presentation.Orchestrator;
using Presentation.Orchestrator.Configurations;
using Presentation.Orchestrator.Workflows;

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

services.AddTransient<IStateMachineConfigBuilder, StateMachineConfigBuilder>();
services.AddSingleton<IWorkflowsManager, WorkflowsManager>();
services.AddSingleton<ApplicationsConfigurations>(GetApplicationsConfigurations());
services.AddInfrastructureServices();
services.AddApplicationServices();

services.AddHostedService<Worker>();

var app = builder.Build();

app.UseCors();
app.MapHub<MonitoringHub>("/hubs/monitoring").RequireCors("corsPolicy1");

app.MapGet("/config", () =>
{
    string path = Path.Combine(Environment.CurrentDirectory, @"appsConfig.json");
    var content = File.ReadAllText(path);
    dynamic configs = JsonSerializer.Deserialize<dynamic>(content);
    return configs;
    
}).RequireCors("corsPolicy1");;

app.Run();

ApplicationsConfigurations GetApplicationsConfigurations()
{
    string path = Path.Combine(Environment.CurrentDirectory, @"appsConfig.json");
    var content = File.ReadAllText(path);
    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };
    var applicationsConfigurations = JsonSerializer.Deserialize<ApplicationsConfigurations>(content, options);
    if (applicationsConfigurations == null)
    {
        throw new InvalidOperationException("Applications configurations could not be loaded.");
    }
    return applicationsConfigurations;
}