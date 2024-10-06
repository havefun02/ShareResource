using ShareResource;
using Serilog;
public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build())
            .CreateLogger();
        var builder = WebApplication.CreateBuilder(args).Build();
        builder.Run();
    }
    public static IHostBuilder CreateBuilde(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureWebHostDefaults(webBuilders => { webBuilders.UseStartup<Startup>(); });
}