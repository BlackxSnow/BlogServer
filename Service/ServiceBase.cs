using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Service;

public class ServiceBase
{
    public WebApplication Application { get; private set; }
    public HttpClient Client { get; private set; } = new HttpClient();

    protected virtual void ConfigureBuilder(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
    }

    protected virtual void ConfigureApplication(WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseHttpLogging();
        app.MapControllers();
    }
    
    public ServiceBase(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        ConfigureBuilder(builder);
        Application = builder.Build();
        
        // ProjectManager = new ProjectManager();
        
        ConfigureApplication(Application);
        
        // ProjectManager.GenerateCache(args.ElementAtOrDefault(1) ?? "/home/celeste/professional/PortfolioGulp/dev/projects");
    }

    public void Run()
    {
        Application.Start();

        string address = Application.Services.GetService<IServer>()!.Features.Get<IServerAddressesFeature>()!.Addresses.First();
        
        Gateway.RegisterToGateway(Client, Application.Configuration, address[..address.IndexOf(':')],
            address[(address.LastIndexOf(':')+1)..]);
        
        Application.WaitForShutdown();
    }
}