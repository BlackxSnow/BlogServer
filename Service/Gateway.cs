using System.Text;
using Microsoft.Extensions.Configuration;
using Utility;

namespace Service;

public static class Gateway
{
    public static void RegisterToGateway(HttpClient client, IConfiguration configuration, string serviceProtocol, string servicePort)
    {
        var serviceId = configuration.GetValueThrow<string>("Gateway:ServiceID");
        var serviceName = configuration.GetValueThrow<string>("Gateway:ServiceName");
        var gatewayAddress = configuration.GetValueThrow<string>("Gateway:Address");
        
        if (!File.Exists($"{serviceId}.token")) throw new FileNotFoundException("No authentication token was found.");
        
        string token = File.ReadAllText($"{serviceId}.token");

        using var message = new HttpRequestMessage(HttpMethod.Post,
            $"{gatewayAddress}/api/service/Register?id={serviceId}&name={serviceName}&port={servicePort}");
        message.Content = new StringContent(token, new UTF8Encoding(), "application/json");
        HttpResponseMessage response = client.Send(message);
        Console.WriteLine(bool.Parse(response.Content.ReadAsStringAsync().Result) ? 
            $"Successfully registered to gateway at {gatewayAddress} with ID {serviceId}, Name {serviceName}." :
            $"Failed to register to gateway at {gatewayAddress}.");
    }
}