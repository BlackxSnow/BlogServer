using System.Text.Json;
using APIGateway.Services.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace APIGateway.Services;

[Route("api/service")]
[ApiController]
public class ServiceController : ControllerBase
{
    [HttpPost("[action]")]
    public bool Register([FromQuery] string id, [FromQuery] string name, [FromQuery] string port, [FromBody] AuthenticationToken authToken)
    {
        if (!Program.Gateway.Authenticator.ActivateToken(authToken) || id != authToken.Id) return false;
        var service = new Service(id, name, HttpContext.Connection.RemoteIpAddress, int.Parse(port),
            authToken, true);
        ServiceRegistry.AddService(service);
        Console.WriteLine($"Service '{id}:{name}' registered at {service.Address}:{service.Port}");
        return true;
    }

    [HttpPost("[action]")]
    public void GenerateToken(string id)
    {
        AuthenticationToken token = Program.Gateway.Authenticator.GenerateToken(id);
        if (!Directory.Exists("tokens")) Directory.CreateDirectory("tokens");
        using FileStream writer = System.IO.File.OpenWrite($"tokens/{id}.token");
        JsonSerializer.Serialize(writer, token);
    }
}