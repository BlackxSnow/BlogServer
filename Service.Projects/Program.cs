using System.Text;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace Projects.Service;

internal static class Program
{
    public static ProjectService ServiceInstance;

    static void Main(string[] args)
    {
        ServiceInstance = new ProjectService(args);
        ServiceInstance.Run();
    }
} 


