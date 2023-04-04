using System.Net;

namespace APIGateway.Services;

public class Service
{
    public readonly string Id;
    public readonly string Name;
    public readonly IPAddress Address;
    public readonly int Port;
    public readonly AuthenticationToken AuthenticationToken;
    public bool Status;

    public Service(string id, string name, IPAddress address, int port, AuthenticationToken token, bool status)
    {
        Id = id;
        Name = name;
        Address = address;
        Port = port;
        AuthenticationToken = token;
        Status = status;
    }
}