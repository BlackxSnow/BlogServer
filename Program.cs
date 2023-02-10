using BlogServer;
using BlogServer.ResourceViews;
using Microsoft.AspNetCore.WebSockets;

namespace BlogServer;

internal static class Program
{
    public static WebApplication App { get; private set; }
    public static PostManager PostManager { get; private set; }
    public static ProjectManager ProjectManager { get; private set; }
    
    static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddRazorPages();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", b => 
                b.WithOrigins("*")
                    .AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed((host) => true));
        });
        builder.Services.AddControllers();
        App = builder.Build();
        

        if (!App.Environment.IsDevelopment())
        {
            App.UseExceptionHandler("/Error");
            App.UseHsts();
        }

        if (args.ElementAtOrDefault(2) != null) App.UseHttpsRedirection();
        App.UseStaticFiles();

        App.UseRouting();
        App.UseCors();
        App.UseHttpLogging();
        App.UseWebSockets();

        App.UseAuthorization();
        App.MapRazorPages();

        PostManager = new PostManager();
        ProjectManager = new ProjectManager();
        
        App.MapControllers();

        PostManager.GeneratePostCache(args.ElementAtOrDefault(0) ?? "/home/celeste/professional/PortfolioGulp/dev");
        ProjectManager.GenerateCache(args.ElementAtOrDefault(1) ?? "/home/celeste/professional/PortfolioGulp/dev/projects");

        App.Run();
    }
} 


