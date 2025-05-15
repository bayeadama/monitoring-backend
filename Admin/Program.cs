// See https://aka.ms/new-console-template for more information

using System.Net.Http.Headers;
using Common;
using RabbitMQ.Client;

Console.WriteLine("Hello, World!");

while (true)
{
    Console.Write("> ");
    string? readLine = Console.ReadLine();
    
    if(readLine == "q")
        break;

    switch (readLine)
    {
        case "info":
            await GetChannelInfo();
            break;
    }
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