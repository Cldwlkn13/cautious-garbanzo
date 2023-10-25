﻿using System;
using System.Collections.Concurrent;
using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Interfaces;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Services
{
    public class MappingService : IMappingService
    {
        public MappingService()
        {
        }

        public KeyValuePair<Event, ConcurrentDictionary<DateTime, IList<MarketBook>>> MapEventToMarketBooks(ConcurrentDictionary<Event, ConcurrentDictionary<DateTime, IList<MarketBook>>> marketBooks, EventWithCompetition @event)
        {
            return marketBooks.FirstOrDefault(e => e.Key.Id == @event.Event.Id);
        }

        public bool TryMapSportsbookMarketDetailsToEvent(IDictionary<EventWithCompetition, IEnumerable<MarketDetail>> sportsbookMarketDetails, EventWithCompetition @event, out IEnumerable<MarketDetail> result)
        {
            if (!sportsbookMarketDetails.ContainsKey(@event))
            {
                result = new List<MarketDetail>();
                return false;
            }

            result = sportsbookMarketDetails[@event];
            return true;
        }

        public bool TryMapMarketsBooksToSportsbookMarketDetail(KeyValuePair<Event, ConcurrentDictionary<DateTime, IList<MarketBook>>> eventWithMarketBooks, MarketDetail marketDetail, out KeyValuePair<DateTime, IList<MarketBook>> result)
        {
            result = new KeyValuePair<DateTime, IList<MarketBook>>();

            if (eventWithMarketBooks.Key == null)
            {
                return false;
            }

            if (!eventWithMarketBooks.Value.ContainsKey(marketDetail.marketStartTime))
            {
                return false;
            }

            var mappedMarketBooks = eventWithMarketBooks.Value.FirstOrDefault(k => k.Key == marketDetail.marketStartTime);

            if (mappedMarketBooks.Value == null)
            {
                return false;
            }

            result = mappedMarketBooks;
            return true;
        }

        public bool TryMapMarketBook(KeyValuePair<DateTime, IList<MarketBook>> marketBooks, int numberOfWinners, out MarketBook result)
        {
            var mappedMarketBook = marketBooks.Value.FirstOrDefault(m => m.NumberOfWinners == numberOfWinners);

            if (mappedMarketBook == null)
            {
                result = new MarketBook();
                return false;
            }

            result = mappedMarketBook;
            return true;
        }

        public bool TryMapMarketBook(KeyValuePair<DateTime, IList<MarketBook>> marketBooks, MarketDetail marketDetail, out MarketBook result)
        {
            var mappedMarketBook = marketBooks.Value.FirstOrDefault(m => m.MarketId == marketDetail.linkedMarketId);

            if (mappedMarketBook == null)
            {
                result = new MarketBook();
                return false;
            }

            result = mappedMarketBook;
            return true;
        }

        public bool TryMapRunner(MarketBook marketBook, RunnerDetail sportsbookRunner, out Runner result)
        {
            var mappedRunner = marketBook?.Runners.FirstOrDefault(r => r.SelectionId == sportsbookRunner.selectionId);

            if (mappedRunner == null)
            {
                result = new Runner();
                return false;
            }

            result = mappedRunner;
            return true;
        }

        public bool TryMapScrapedEvent(List<ScrapedEvent> scrapedEvents, EventWithCompetition ewc, MarketDetail md, out ScrapedEvent result)
        {
            var mappedEvent = scrapedEvents.FirstOrDefault(s =>
                     s.MappedEvent.EventWithCompetition.Event.Venue == ewc.Event.Venue &&
                     s.MappedEvent.SportsbookMarket.marketStartTime == md.marketStartTime);

            if (mappedEvent == null)
            {
                result = new ScrapedEvent();
                return false;
            }

            result = mappedEvent;
            return true;
        }

        public bool TryMapScrapedMarket(ScrapedEvent scrapedEvent, out ScrapedMarket result)
        {
            result = new ScrapedMarket();

            if (!scrapedEvent.ScrapedMarkets.Any())
            {
                return false;
            }

            result = scrapedEvent.ScrapedMarkets.First();
            return true;
        }

        public bool TryMapScrapedRunner(ScrapedMarket scrapedMarket, RunnerDetail sportsbookRunner, out ScrapedRunner result)
        {
            var mappedRunner = scrapedMarket?.ScrapedRunners.FirstOrDefault(r =>
                  r.Name.ToLower().Replace("'", "") == sportsbookRunner.selectionName.ToLower().Replace("'", ""));

            if (mappedRunner == null)
            {
                result = new ScrapedRunner();
                return false;
            }

            result = mappedRunner;
            return true;
        }
    }
}

