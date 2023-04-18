using System.Net.Http.Headers;
using APIGateway.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Yarp.ReverseProxy.Forwarder;

namespace APIGateway;

public class RouterMiddleware
{
    private readonly RequestDelegate _Next;
    private ILogger<RouterMiddleware> _Logger;
    
    public RouterMiddleware(RequestDelegate next, ILogger<RouterMiddleware> logger)
    {
        _Next = next;
        _Logger = logger;
    }

    public async Task<IActionResult> InvokeAsync(HttpContext context, IHttpForwarder forwarder)
    {
        if (context.GetEndpoint() != null)
        {
            _Logger.Log(LogLevel.Debug, $"No endpoint for request. {context.Request.GetDisplayUrl()}");
            await _Next(context);
            return new UnauthorizedResult();
        }

        string[] path = context.Request.Path.ToString().Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (path.Length < 2 || path[0] != "api" || !ServiceRegistry.TryGetService(path[1], out Service? service))
        {
            _Logger.Log(LogLevel.Information, $"Unknown or invalid routing request: {context.Request.GetDisplayUrl()}");
            await _Next(context);
            return new UnauthorizedResult();
        }
        
        // using var message = new HttpRequestMessage(new HttpMethod(context.Request.Method),
        //     $"http://{service.Address}:{service.Port}{context.Request.Path}{context.Request.QueryString}");
        // message.Content = new StreamContent(context.Request.Body);
        // message.Content.Headers.ContentType = new MediaTypeHeaderValue(context.Request.Headers.ContentType.ToString());
        //
        // HttpResponseMessage response = await Program.Gateway.Client.SendAsync(message);
        var result =
            await forwarder.SendAsync(context, $"http://{service.Address}:{service.Port}", Program.Gateway.Client);
        return new OkResult();
        // context.Response.ContentType = response.Content.Headers.ContentType?.MediaType;
        // context.Response.StatusCode = (int)response.StatusCode;
        // await response.Content.CopyToAsync(context.Response.Body);
    }
    
    
}

public static class RouterMiddlewareExtensions
{
    public static IApplicationBuilder UseRouterMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RouterMiddleware>();
    }
}