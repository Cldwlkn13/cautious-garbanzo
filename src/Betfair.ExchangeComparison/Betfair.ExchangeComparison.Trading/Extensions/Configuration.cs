using Betfair.ExchangeComparison.Interfaces;
using Betfair.ExchangeComparison.Trading;
using Microsoft.Extensions.DependencyInjection;

namespace Betfair.ExchangeComparison.Configurations
{
    public static partial class Configuration
    {
        public static void ConfigureTrading(this IServiceCollection services)
        {
            services.AddSingleton<ITradingHandler, TradingHandler>();
        }
    }
}
