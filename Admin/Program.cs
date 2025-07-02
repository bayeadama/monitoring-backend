// See https://aka.ms/new-console-template for more information

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Admin;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);
var services = builder.Services;
services.AddHostedService<Worker>();
//builder.Services.AddSignalR();
var host = builder.Build();
host.Run();


while (true)
{
    
    Thread.Sleep(1000);
}

static async Task GetChannelInfo()
{

    using (var httpClient = new HttpClient())
    {
        var userName = "guest";
        var userPassword = "guest";
        var authenticationString = $"{userName}:{userPassword}";
        var base64String = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(authenticationString));

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64String);

        //httpClient.BaseAddress = new Uri("http://localhost:15666");
        string url = "http://localhost:15666/api/exchanges/%2F/main.monitoring.fanout/bindings/source";
        string response = await httpClient.GetStringAsync(url);
        
        Console.WriteLine(response);
    }
    
}