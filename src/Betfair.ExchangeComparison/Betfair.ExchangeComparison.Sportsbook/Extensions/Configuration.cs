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
            services.AddSingleton<IBetfairSportsbookHandler, BetfairSportsbookHandler>();
            services.AddSingleton<IPaddyPowerSportsbookHandler, PaddyPowerSportsbookHandler>();
            services.AddSingleton<ISportsbookClient, SportsbookClient>();
        }
    }
}

