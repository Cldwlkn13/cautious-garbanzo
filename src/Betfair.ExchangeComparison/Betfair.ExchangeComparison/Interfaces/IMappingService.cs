﻿using System.Collections.Concurrent;
using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Matchbook;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Interfaces
{
    public interface IMappingService
    {
        KeyValuePair<Event, ConcurrentDictionary<DateTime, IList<MarketBook>>> MapEventToMarketBooks(ConcurrentDictionary<Event, ConcurrentDictionary<DateTime, IList<MarketBook>>> marketBooks, EventWithCompetition @event);
        EventWithMarketBooks MapEventToMarketBooksObj(ConcurrentDictionary<Event, ConcurrentDictionary<DateTime, IList<MarketBook>>> marketBooks, EventWithCompetition @event);
        
        bool TryMapMarketsBooksToSportsbookMarketDetail(KeyValuePair<Event, ConcurrentDictionary<DateTime, IList<MarketBook>>> eventWithMarketBooks, MarketDetail marketDetail, out KeyValuePair<DateTime, IList<MarketBook>> result);
        bool TryMapMarketsBooksToSportsbookMarketDetailObj(EventWithMarketBooks eventWithMarketBooks, MarketDetail marketDetail, out MarketBooksByDateTime result);
        
        bool TryMapSportsbookMarketDetailsToEvent(IDictionary<EventWithCompetition, IEnumerable<MarketDetail>> sportsbookMarkets, EventWithCompetition @event, out IEnumerable<MarketDetail> result);

        bool TryMapMarketBook(KeyValuePair<DateTime, IList<MarketBook>> marketBooks, int numberOfWinners, out MarketBook result);
        bool TryMapMarketBook(KeyValuePair<DateTime, IList<MarketBook>> marketBooks, MarketDetail marketDetail, out MarketBook result);
        bool TryMapMarketBook(MarketBooksByDateTime marketBooks, int numberOfWinners, out MarketBook result);
        bool TryMapMarketBook(MarketBooksByDateTime marketBooks, MarketDetail marketDetail, out MarketBook result);

        bool TryMapRunner(MarketBook marketBook, RunnerDetail sportsbookRunner, out Runner result);

        bool TryMapScrapedEvent(List<ScrapedEvent> scrapedEvents, EventWithCompetition ewc, MarketDetail md, out ScrapedEvent result);
        bool TryMapScrapedEvent(List<ScrapedEvent> scrapedEvents, EventWithCompetition ewc, out ScrapedEvent result);

        bool TryMapScrapedMarket(ScrapedEvent scrapedEvent, out ScrapedMarket result);
        bool TryMapScrapedMarket(ScrapedEvent scrapedEvent, MarketDetail marketDetail, out ScrapedMarket result);
        bool TryMapScrapedRunner(ScrapedMarket scrapedMarket, RunnerDetail sportsbookRunner, out ScrapedRunner result);

        bool TryMapMatchbookEvents(List<MatchbookEvent> matchbookEvents, EventWithCompetition ewc, out List<MatchbookEvent> result);
        bool TryMapMatchbookEventToMarketDetail(List<MatchbookEvent> matchbookEvents, MarketDetail marketDetail, out MatchbookEvent result);
    }
}

