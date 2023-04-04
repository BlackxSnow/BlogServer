using System.Net.Http.Headers;
using APIGateway.Services;

namespace APIGateway;

public class RouterMiddleware
{
    private readonly RequestDelegate _Next;
    
    public RouterMiddleware(RequestDelegate next)
    {
        _Next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.GetEndpoint() != null)
        {
            Console.WriteLine("Route bailed 1.");
            await _Next(context);
            return;
        }

        string[] path = context.Request.Path.ToString().Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (path.Length < 2 || path[0] != "api" || !ServiceRegistry.TryGetService(path[1], out Service? service))
        {
            Console.WriteLine("Route bailed 2.");
            await _Next(context);
            return;
        }

        using var message = new HttpRequestMessage(new HttpMethod(context.Request.Method),
            $"http://{service.Address}:{service.Port}{context.Request.Path}{context.Request.QueryString}");
        message.Content = new StreamContent(context.Request.Body);

        HttpResponseMessage response = await Program.Gateway.Client.SendAsync(message);
        context.Response.ContentType = response.Content.Headers.ContentType?.MediaType;
        context.Response.StatusCode = (int)response.StatusCode;
        await response.Content.CopyToAsync(context.Response.Body);
    }
    
    
}

public static class RouterMiddlewareExtensions
{
    public static IApplicationBuilder UseRouterMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RouterMiddleware>();
    }
}