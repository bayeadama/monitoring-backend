using Application;
using Infrastructure;
using Presentation.Commander;

var builder = Host.CreateApplicationBuilder(args);

var services = builder.Services;

services.AddInfrastructureServices();
services.AddApplicationServices();

services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();