using Betfair.ExchangeComparison.Configurations;
using Betfair.ExchangeComparison.Domain.Settings;
using Betfair.ExchangeComparison.Interfaces;
using Betfair.ExchangeComparison.Trading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Betfair.ExchangeComparison.Tests.Integration
{
    public class AbstractTests
    {
        protected IServiceProvider? Services;
        protected IConfiguration? Configuration;

        protected AbstractTests()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddUserSecrets(Assembly.GetAssembly(typeof(AbstractTests)))
                .Build();

            RegisterServices();
        }

        private void RegisterServices()
        {
            var services = new ServiceCollection();

            //add core services
            services.AddLogging();

            //add projects
            services.ConfigureMatchbook();

            services.AddSingleton<ITradingHandler, TradingHandler>();

            //bind settings
            services.Configure<MatchbookSettings>(o =>
                Configuration!.GetSection("MatchbookSettings").Bind(o));

            //build
            Services = services.BuildServiceProvider();
        }
    }
}
