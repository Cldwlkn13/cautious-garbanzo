using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Domain.DomainModel
{
    public class SportsbookCatalogue
    {
        public SportsbookCatalogue()
        {
            EventsWithMarketCatalogue = new Dictionary<EventWithCompetition, IEnumerable<MarketCatalogue>>();
            EventsWithMarketDetails = new Dictionary<EventWithCompetition, IEnumerable<MarketDetail>>();
        }

        public IDictionary<EventWithCompetition, IEnumerable<MarketCatalogue>> EventsWithMarketCatalogue { get; set; }
        public IDictionary<EventWithCompetition, IEnumerable<MarketDetail>> EventsWithMarketDetails { get; set; }
    }
}

