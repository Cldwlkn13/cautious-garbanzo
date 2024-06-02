using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.Settings;
using Betfair.ExchangeComparison.Domain.TradingModel;
using Betfair.ExchangeComparison.Pages.Models;
using Microsoft.Extensions.Options;

namespace Betfair.ExchangeComparison.Trading.TradingFilters
{
    public static class RaceMetaFilters
    {
        public static bool FilterByRaceDistance(BestRunner bestRunner, MyMarketPosition marketPosition, IOptions<TradingSettings> settings, List<string> LoggingStore)
        {
            if (bestRunner.MarketMeta.Furlongs <= 6)
            {
                ConsoleExtensions.WriteLine($"{nameof(FilterByRaceDistance)} " +
                    $"{bestRunner.FiltersLogRunner()} " +
                    $"IsFlatSprint=true",
                    ConsoleColor.DarkMagenta, LoggingStore);
                return false;
            }

            return true;
        }
    }
}
