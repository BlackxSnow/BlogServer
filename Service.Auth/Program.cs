using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auth.Service;

internal static class Program
{
    public static AuthService ServiceInstance;
    static void Main(string[] args)
    {
        ServiceInstance = new AuthService(args);
        ServiceInstance.Run();

        
    }
}