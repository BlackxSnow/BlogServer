namespace BlogServer.Utility;

public static class Logging
{
    // private static ILoggerFactory _LoggerFactory { get; set; } = LoggerFactory.Create(builder =>
    // {
    //     builder.AddConsole();
    //     builder.AddDebug();
    //     
    // });

    public static ILogger CreateLogger<T>() => Program.App.Services.GetService<ILogger<T>>();
    // public static ILogger CreateLogger(string category) => _LoggerFactory.CreateLogger(category);
    
    
}