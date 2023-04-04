using APIGateway.Services;
using APIGateway.Services.Authentication;

namespace APIGateway;

internal static class Program
{
    public static Gateway Gateway { get; private set; }
    static void Main(string[] args)
    {
        var authenticator = new TokenAuthenticator();
        Gateway = new Gateway(authenticator, args);
        Gateway.Application.Run();
    }
    
}