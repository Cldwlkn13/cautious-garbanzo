using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.Settings;
using Betfair.ExchangeComparison.Domain.TradingModel;
using Betfair.ExchangeComparison.Pages.Models;
using Microsoft.Extensions.Options;

namespace Betfair.ExchangeComparison.Trading.TradingFilters
{
    public static class MarketLimitFilters
    {
        public static bool FilterByMaxMarketLiability(BestRunner bestRunner, MyMarketPosition marketPosition, IOptions<TradingSettings> settings, List<string> LoggingStore)
        {
            if (marketPosition.MaxLoss > settings.Value.MaxRaceLiability) //have we exceeded max loss on the market already?
            {
                ConsoleExtensions.WriteLine($"{nameof(FilterByMaxMarketLiability)} " +
                    $"{bestRunner.FiltersLogRunner()} " +
                    $"Max Race Liability Exceeded {marketPosition.MaxLoss}",
                    ConsoleColor.DarkMagenta, LoggingStore);
                return false;
            }
            return true;
        }
    }
}
