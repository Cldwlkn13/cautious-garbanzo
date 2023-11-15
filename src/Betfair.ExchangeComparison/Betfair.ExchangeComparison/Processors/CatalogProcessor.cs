using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Interfaces;
using Betfair.ExchangeComparison.Pages.Model;

namespace Betfair.ExchangeComparison.Processors
{
    public class CatalogProcessor
    {
        private readonly ICatalogService _catalogService;

        public CatalogProcessor(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        public async Task<BaseCatalogModel> Process()
        {
            var t1 = _catalogService.GetSportsbookCatalogue(
                Sport.Football,
                BetfairQueryExtensions.TimeRangeForNextDays(1), Bookmaker.BetfairSportsbook);

            var t2 = _catalogService.GetExchangeCatalogue(
                Sport.Football,
                BetfairQueryExtensions.TimeRangeForNextDays(1));

            await Task.WhenAll(new Task[] { t1, t2 });

            var sportsbookCatalogue = t1.Result;
            var exchangeCatalogue = t2.Result;

            return new BaseCatalogModel()
            {
                SportsbookCatalogue = sportsbookCatalogue,
                ExchangeCatalogue = exchangeCatalogue
            };
        }
    }
}

