using System.Net.Http.Headers;
using System.Text;
using BlogServer;
using BlogServer.ResourceViews;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.WebSockets;

namespace BlogServer;

internal static class Program
{
    public static BlogService ServiceInstance;
    
    static void Main(string[] args)
    {
        ServiceInstance = new BlogService(args);
        ServiceInstance.Run();
        // WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        //
        // // Add services to the container.
        //
        // builder.Services.AddRazorPages();
        // builder.Services.AddCors(options =>
        // {
        //     options.AddPolicy("CorsPolicy", b => 
        //         b.WithOrigins("*")
        //             .AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed((host) => true));
        // });
        // builder.Services.AddControllers();
        // App = builder.Build();
        //
        //
        // if (!App.Environment.IsDevelopment())
        // {
        //     App.UseExceptionHandler("/Error");
        //     App.UseHsts();
        // }
        //
        // if (args.ElementAtOrDefault(2) != null) App.UseHttpsRedirection();
        // App.UseStaticFiles();
        //
        // App.UseRouting();
        // App.UseCors();
        // App.UseHttpLogging();
        // App.UseWebSockets();
        //
        // App.UseAuthorization();
        // App.MapRazorPages();
        //
        // PostManager = new PostManager();
        //
        // App.MapControllers();
        //
        // PostManager.GeneratePostCache(args.ElementAtOrDefault(0) ?? "/home/celeste/professional/PortfolioGulp/dev");
        // // ProjectManager.GenerateCache(args.ElementAtOrDefault(1) ?? "/home/celeste/professional/PortfolioGulp/dev/projects");
        //
        //
        //
        // App.Start();
        //
        // string address = App.Services.GetService<IServer>()!.Features.Get<IServerAddressesFeature>()!.Addresses.First();
        //
        // Utility.Service.RegisterToGateway(Client, App.Configuration, address[..address.IndexOf(':')],
        //     address[(address.LastIndexOf(':')+1)..]);
        //
        // App.WaitForShutdown();
    }
} 


