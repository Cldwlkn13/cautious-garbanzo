using System;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Domain.DomainModel
{
    public class SportsbookCatalogue
    {
        public SportsbookCatalogue()
        {
        }

        public IDictionary<EventWithCompetition, IEnumerable<MarketCatalogue>> EventsWithMarketCatalogue { get; set; }
        public IDictionary<EventWithCompetition, IEnumerable<MarketDetail>> EventsWithMarketDetails { get; set; }
    }
}

