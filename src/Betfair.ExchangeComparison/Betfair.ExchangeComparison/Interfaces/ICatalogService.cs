using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Interfaces
{
    public interface ICatalogService
    {
        Task<SportsbookCatalogue> GetSportsbookCatalogue(Sport sport, TimeRange? timeRange = null, Bookmaker bookmaker = Bookmaker.BetfairSportsbook, int addDays = 1);
        Task<ExchangeCatalogue> GetExchangeCatalogue(Sport sport, TimeRange? timeRange = null);

        IEnumerable<MarketDetailWithEwc> UpdateMarketDetailCatalog(Sport sport, int addDays = 1);
        Dictionary<EventWithCompetition, List<MarketDetail>> UpdateMarketDetailCatalogGroupByEvent(Sport sport, int addDays = 1);

        public Dictionary<DateTime, Dictionary<Sport, Dictionary<string, Event>>> ExchangeEventStore { get; }
        public Dictionary<DateTime, Dictionary<Sport, IEnumerable<MarketCatalogue>>> ExchangeMarketCatalogueStore { get; }

        public Dictionary<DateTime, Dictionary<Sport, IDictionary<EventWithCompetition, IEnumerable<MarketCatalogue>>>> SportsbookMarketCatalogueStore { get; }
        public Dictionary<DateTime, Dictionary<Sport, IEnumerable<MarketDetailWithEwc>>> SportsbookMarketDetailsStore { get; }
    }
}

