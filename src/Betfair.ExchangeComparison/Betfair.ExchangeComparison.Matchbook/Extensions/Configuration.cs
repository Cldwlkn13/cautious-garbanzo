using Betfair.ExchangeComparison.Matchbook;
using Betfair.ExchangeComparison.Matchbook.Clients;
using Betfair.ExchangeComparison.Matchbook.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Betfair.ExchangeComparison.Configurations
{
    public static partial class Configuration
    {
        public static void ConfigureMatchbook(this IServiceCollection services)
        {
            services.AddHttpClient<ISessionClient, SessionClient>();
            services.AddHttpClient<IAccountClient, AccountClient>();
            services.AddHttpClient<ICatalogueClient, CatalogueClient>();
            services.AddHttpClient<IBettingClient, BettingClient>();
            services.AddHttpClient<IReportsClient, ReportsClient>();

            services.AddSingleton<IMatchbookHandler, MatchbookHandler>();   
        }
    }
}
