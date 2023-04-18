using System.Text;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Utility;

namespace Service;

public class Gateway : IHostedService
{
    private HttpClient _HttpClient { get; }
    private IHostApplicationLifetime _ApplicationLifetime { get; }
    private IServer _Server { get; }
    private GatewayOptions _Options { get; }

    public Gateway(HttpClient client, IServer server, IHostApplicationLifetime lifetime, IOptions<GatewayOptions> options)
    {
        _HttpClient = client;
        _ApplicationLifetime = lifetime;
        _Server = server;
        _Options = options.Value;
    }

    private void Register()
    {
        if (!File.Exists($"{_Options.ServiceID}.token")) throw new FileNotFoundException("No authentication token was found.");
        string token = File.ReadAllText($"{_Options.ServiceID}.token");

        string address = _Server.Features.Get<IServerAddressesFeature>()!.Addresses!.First();
        string port = address[(address.LastIndexOf(':') + 1)..];
        
        using var message = new HttpRequestMessage(HttpMethod.Post,
            $"{_Options.Address}/api/service/Register?id={_Options.ServiceID}&name={_Options.ServiceName}&port={port}");
        message.Content = new StringContent(token, new UTF8Encoding(), "application/json");
        HttpResponseMessage response = _HttpClient.Send(message);
        Console.WriteLine(bool.Parse(response.Content.ReadAsStringAsync().Result) ? 
            $"Successfully registered to gateway at {_Options.Address} with ID {_Options.ServiceID}, Name {_Options.ServiceName}." :
            $"Failed to register to gateway at {_Options.Address}.");
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ApplicationLifetime.ApplicationStarted.Register(Register);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

public class GatewayOptions
{
    public const string Key = "Gateway";

    public string ServiceName { get; set; } = string.Empty;
    public string ServiceID { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}