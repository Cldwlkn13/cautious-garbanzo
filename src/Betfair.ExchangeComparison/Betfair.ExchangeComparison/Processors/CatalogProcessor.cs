using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Interfaces;
using Betfair.ExchangeComparison.Pages.Model;

namespace Betfair.ExchangeComparison.Processors
{
    public class CatalogProcessor : ICatalogProcessor
    {
        private readonly ICatalogService _catalogService;

        public CatalogProcessor(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        public async Task<BaseCatalogModel?> Process(Sport sport)
        {
            try
            {         
                var t1 = _catalogService.GetSportsbookCatalogue(
                    sport,
                    BetfairQueryExtensions.TimeRangeForNextDays(1), Bookmaker.BetfairSportsbook);

                var t2 = _catalogService.GetExchangeCatalogue(
                    sport,
                    BetfairQueryExtensions.TimeRangeForNextDays(1));

                var t3 = _catalogService.GetMatchbookCatalogue(
                    sport,
                    BetfairQueryExtensions.TimeRangeForNextDays(1));

                await Task.WhenAll(new Task[] { t1, t2, t3 });

                var sportsbookCatalogue = t1.Result;
                var exchangeCatalogue = t2.Result;
                var matchbookCatalogue = t3.Result;

                return new BaseCatalogModel()
                {
                    SportsbookCatalogue = sportsbookCatalogue,
                    ExchangeCatalogue = exchangeCatalogue,
                    MatchbookCatalogue = matchbookCatalogue,
                    HasEachWay = sport == Sport.Racing ? true : false
                };
            }
            catch (APINGException apiException)
            {
                Console.WriteLine($"APING_EXCEPTION; " +
                    $"Exception={apiException.Message};" +
                    $"ErrorCode={apiException.ErrorCode};");

                return null;
            }
            catch (System.Exception exception)
            {
                Console.WriteLine($"CATALOG_BUILD_EXCEPTION; " +
                    $"Exception={exception.Message}");

                return null;
            }
        }
    }
}

