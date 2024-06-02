using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.Settings;
using Betfair.ExchangeComparison.Domain.TradingModel;
using Betfair.ExchangeComparison.Pages.Models;
using Microsoft.Extensions.Options;

namespace Betfair.ExchangeComparison.Trading.TradingFilters
{
    public static class MarketStateFilters
    {
        public static bool FilterByMarketOverround(BestRunner bestRunner, MyMarketPosition marketPosition, IOptions<TradingSettings> settings, List<string> LoggingStore)
        {
            if(marketPosition.MatchbookMarket.BackOverround > 105)
            {
                ConsoleExtensions.WriteLine($"{nameof(FilterByMarketOverround)} " +
                    $"{bestRunner.Event.Venue} {bestRunner.MarketDetail.marketStartTime.ConvertUtcToBritishIrishLocalTime().ToString("HH:mm")} " +
                    $"- {bestRunner} " +
                    $"{bestRunner.SportsbookRunner.winRunnerOdds.@decimal:F} - " +
                    $"Race Back Overround > 105",
                    ConsoleColor.DarkMagenta, LoggingStore);

                return false;
            }

            else if ((marketPosition.MatchbookMarket.BackOverround - marketPosition.MatchbookMarket.LayOverround) > 8)
            {
                ConsoleExtensions.WriteLine($"{nameof(FilterByMarketOverround)} " +
                    $"{bestRunner.FiltersLogRunner()} " +
                    $"Race Overround Spread > 8",
                    ConsoleColor.DarkMagenta, LoggingStore);

                return false;
            }

            return true;
        }
    }
}
