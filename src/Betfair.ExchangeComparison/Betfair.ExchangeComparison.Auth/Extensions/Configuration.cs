using Betfair.ExchangeComparison.Auth;
using Betfair.ExchangeComparison.Auth.Clients;
using Betfair.ExchangeComparison.Auth.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Betfair.ExchangeComparison.Configurations
{
    public static partial class Configuration
    {
        public static void ConfigureAuth(this IServiceCollection services)
        {
            services.AddTransient<IAuthClient, AuthClient>();
            services.AddTransient<IAuthHandler, AuthHandler>();
        }
    }
}

