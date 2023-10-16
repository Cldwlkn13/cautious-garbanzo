using Betfair.ExchangeComparison.Exchange;
using Betfair.ExchangeComparison.Exchange.Clients;
using Betfair.ExchangeComparison.Exchange.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Betfair.ExchangeComparison.Configurations
{
    public static partial class Configuration
    {
        public static void ConfigureExchange(this IServiceCollection services)
        {
            services.AddTransient<IAuthClient, AuthClient>();
            services.AddSingleton<IExchangeHandler, ExchangeHandler>();
            services.AddSingleton<IExchangeClient, ExchangeClient>();
        }
    }
}

