using System.Diagnostics.CodeAnalysis;

namespace APIGateway.Services;

public static class ServiceRegistry
{
    private static readonly Dictionary<string, List<Service>> _Services = new Dictionary<string, List<Service>>();

    public static void AddService(Service service)
    {
        if (!_Services.TryGetValue(service.Name, out List<Service>? services))
        {
            services = new List<Service>();
            _Services.Add(service.Name, services);
        }
        services.Add(service);
    }
    
    public static bool TryGetService(string name, [NotNullWhen(true)] out Service? service)
    {
        service = null;
        if (!_Services.TryGetValue(name, out List<Service>? services)) return false;
        service = services.FirstOrDefault(s => s.Status);
        return service != null;
    }
}