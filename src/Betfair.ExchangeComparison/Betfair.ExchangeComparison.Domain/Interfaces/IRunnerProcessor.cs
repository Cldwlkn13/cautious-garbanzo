using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Matchbook;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Pages.Model;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Interfaces
{
    public interface IRunnerProcessor
    {
        Task<RunnerPriceOverview?> Process(BasePageModel basePageModel, EventWithCompetition @event, MarketBook mappedWinMarketBook,
            MarketDetail marketDetail, RunnerDetail sportsbookRunner, bool hasEachWay, ScrapedEvent? mappedScrapedEvent = null, 
            ScrapedMarket? mappedScrapedMarket = null, MarketBook? mappedPlaceMarketBook = null, MatchbookMarket? mappedMatchbookMarket = null);
    }
}
