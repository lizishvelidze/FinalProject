using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using test4;

public class Program
{
    public static void Main(string[] args)
    {
        // Configure Serilog before creating the host
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()        
            .WriteTo.File("logs/myapp.log", rollingInterval: RollingInterval.Day)  
            .CreateLogger();

        try
        {
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application failed to start.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .UseSerilog(); 
}
