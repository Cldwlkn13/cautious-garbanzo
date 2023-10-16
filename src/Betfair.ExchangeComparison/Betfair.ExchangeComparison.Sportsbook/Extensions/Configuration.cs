using Betfair.ExchangeComparison.Sportsbook;
using Betfair.ExchangeComparison.Sportsbook.Clients;
using Betfair.ExchangeComparison.Sportsbook.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Betfair.ExchangeComparison.Configurations
{
    public static partial class Configuration
    {
        public static void ConfigureSportsbook(this IServiceCollection services)
        {
            services.AddSingleton<ISportsbookHandler, SportsbookHandler>();
            services.AddSingleton<ISportsbookClient, SportsbookClient>();
        }
    }
}

