using System;
using System.Collections.Concurrent;
using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Interfaces
{
    public interface IMappingService
    {
        KeyValuePair<Event, ConcurrentDictionary<DateTime, IList<MarketBook>>> MapEventToMarketBooks(ConcurrentDictionary<Event, ConcurrentDictionary<DateTime, IList<MarketBook>>> marketBooks, EventWithCompetition @event);

        bool TryMapSportsbookMarketDetailsToEvent(IDictionary<EventWithCompetition, IEnumerable<MarketDetail>> sportsbookMarkets, EventWithCompetition @event, out IEnumerable<MarketDetail> result);

        bool TryMapMarketsBooksToSportsbookMarketDetail(KeyValuePair<Event, ConcurrentDictionary<DateTime, IList<MarketBook>>> eventWithMarketBooks, MarketDetail marketDetail, out KeyValuePair<DateTime, IList<MarketBook>> result);

        bool TryMapMarketBook(KeyValuePair<DateTime, IList<MarketBook>> marketBooks, int numberOfWinners, out MarketBook result);

        bool TryMapRunner(MarketBook marketBook, RunnerDetail sportsbookRunner, out Runner result);

        bool TryMapScrapedEvent(List<ScrapedEvent> scrapedEvents, EventWithCompetition ewc, MarketDetail md, out ScrapedEvent result);

        bool TryMapScrapedMarket(ScrapedEvent scrapedEvent, out ScrapedMarket result);

        bool TryMapScrapedRunner(ScrapedMarket scrapedMarket, RunnerDetail sportsbookRunner, out ScrapedRunner result);

    }
}

