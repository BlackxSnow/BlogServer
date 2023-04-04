using Microsoft.Extensions.Configuration;

namespace Utility;

public static class ConfigExtensions
{
    public static T GetValueThrow<T>(this IConfiguration config, string key)
    {
        return config.GetValue<T>(key) ??
               throw new KeyNotFoundException($"Configuration value for {key} not found");
    }
}