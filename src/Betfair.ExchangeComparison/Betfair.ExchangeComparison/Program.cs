using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Betfair.ExchangeComparison.Data;

namespace Betfair.ExchangeComparison;

public class Program
{
    public static void Main(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var pathToContentRoot = Directory.GetCurrentDirectory();

        AppDomain currentDomain = AppDomain.CurrentDomain;
        currentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandlerExceptionHandler);

        var configuration = new ConfigurationBuilder()
            .SetBasePath(pathToContentRoot)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .AddUserSecrets(typeof(Program).Assembly)
            .AddCommandLine(args)
            .Build();

        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return new HostBuilder().ConfigureWebHost(webBuilder =>
        {
            webBuilder
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                    config.AddUserSecrets(typeof(Program).Assembly);
                });
        })
        .UseDefaultServiceProvider((context, options) =>
        {
            options.ValidateScopes = true;
        });
    }

    static void UnhandlerExceptionHandler(object sender, UnhandledExceptionEventArgs args)
    {
        Exception e = (Exception)args.ExceptionObject;
        Console.WriteLine("MyHandler caught : " + e.Message);
        Console.WriteLine("Runtime terminating: {0}", args.IsTerminating);
    }
}



