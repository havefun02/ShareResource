using ShareResource;
using Serilog;
using ShareResource.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build())
            .CreateLogger();
        var builder = CreateBuilder(args).Build();
        using (var scope = builder.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            if (context == null) {
                throw new Exception("Service not found");
            }

            Seeder.Seed(context);

        }
        builder.Run();
    }
    public static IHostBuilder CreateBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureWebHostDefaults(webBuilders => { webBuilders.UseStartup<Startup>(); });
}