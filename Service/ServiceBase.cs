using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Identity;
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

        builder.Services.AddDataProtection().SetApplicationName("CelesteWebApp");
        builder.Services.AddAuthentication("Identity.Application")
            .AddCookie("Identity.Application", options =>
        {
            options.Cookie.Name = ".Celeste.SharedCookie";
            // options.LoginPath = ""
        });
        builder.Services.AddAuthorization();
        builder.Services.AddHttpClient();
        builder.Services.AddHostedService<Gateway>();
        builder.Services.Configure<GatewayOptions>(builder.Configuration.GetSection(GatewayOptions.Key));
    }

    protected virtual void ConfigureApplication(WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseAuthentication();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        app.UseHttpLogging();
        app.MapControllers();
    }
    
    public ServiceBase(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        ConfigureBuilder(builder);
        Application = builder.Build();
        
        ConfigureApplication(Application);
    }

    public void Run()
    {
        Application.Start();

        Application.WaitForShutdown();
    }

    public void Start()
    {
        Application.Start();
    }
}