using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service;

namespace Auth.Service;

public class AuthService : ServiceBase
{
    protected override void ConfigureBuilder(WebApplicationBuilder builder)
    {
        // base.ConfigureBuilder(builder);
        builder.Services.AddControllers();
        builder.Services.AddDataProtection().SetApplicationName("CelesteWebApp");
        string connectionString = builder.Configuration.GetConnectionString("AuthContextConnection");
        builder.Services.AddDbContext<UserContext>(opts =>
            opts.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
        
        builder.Services.AddIdentity<User, IdentityRole<Guid>>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<UserContext>();

        builder.Services.ConfigureApplicationCookie(opts =>
        {
            opts.Cookie.Name = ".Celeste.SharedCookie";
            opts.LoginPath = "/api/auth/user/cookieauthenticate";
        });
        builder.Services.AddAuthorization();
        builder.Services.AddHttpClient();
        builder.Services.AddHostedService<Gateway>();
        builder.Services.Configure<GatewayOptions>(builder.Configuration.GetSection(GatewayOptions.Key));
    }

    // protected override void ConfigureApplication(WebApplication app)
    // {
    //     base.ConfigureApplication(app);
    //     using var scope = app.Services.CreateScope();
    //     var context = scope.ServiceProvider.GetRequiredService<UserContext>();
    //
    //     context.users.Add(new User() { Id = "anijandals", Email = "test@test.com", UserName = "usertest"});
    //     context.SaveChanges();
    // }

    public AuthService(string[] args) : base(args)
    {
    }
}