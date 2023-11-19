using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Pages.Model;
using Betfair.ExchangeComparison.Pages.Models;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Interfaces
{
    public interface IMarketProcessor
    {
        Task<MarketViewModel?> Process(BasePageModel basePageModel, EventWithCompetition ewc, MarketDetail marketDetail, 
            EventWithMarketBooks eventWithMarketBooks, bool hasEachWay, ScrapedEvent? mappedScrapedEvent = null);
    }
}
