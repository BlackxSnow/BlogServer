using APIGateway.Services;

namespace APIGateway;

public class Gateway
{
    public readonly HttpClient Client = new HttpClient();
    public WebApplication Application { get; private init; }

    public readonly IAuthenticator Authenticator;

    public Gateway(IAuthenticator authenticator, string[] args)
    {
        Authenticator = authenticator;
        
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.AddCors(c => c.AddDefaultPolicy(p => p.AllowAnyOrigin()));

        Application = builder.Build();
        
        Application.MapControllers();

        Application.UseRouting().UseCors().UseEndpoints(_ => { }).UseMiddleware<RouterMiddleware>();
    }

}