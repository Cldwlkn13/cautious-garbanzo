using System.Collections.Concurrent;
using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Interfaces;
using Betfair.ExchangeComparison.Scraping.Boylesports.Interfaces;

namespace Betfair.ExchangeComparison.Handlers
{
    public class ScrapingHandler : IScrapingHandler
    {
        private readonly IBoylesportsHandler _boylesportsHandler;

        public ScrapingHandler(IBoylesportsHandler boylesportsHandler)
        {
            _boylesportsHandler = boylesportsHandler;

            ScrapedEventsCatalog = new ConcurrentDictionary<Bookmaker, Dictionary<DateTime, Dictionary<string, ScrapedEvent>>>();
        }

        public ConcurrentDictionary<Bookmaker, Dictionary<DateTime, Dictionary<string, ScrapedEvent>>> ScrapedEventsCatalog { get; set; }

        public async Task Handle(IEnumerable<CompoundEventWithMarketDetail> catalog)
        {
            if (!ScrapedEventsCatalog.ContainsKey(Bookmaker.Boylesports))
            {
                ScrapedEventsCatalog.AddOrUpdate(Bookmaker.Boylesports, new Dictionary<DateTime, Dictionary<string, ScrapedEvent>>(),
                    (k, v) => v = new Dictionary<DateTime, Dictionary<string, ScrapedEvent>>());
            }

            foreach (var @event in catalog)
            {
                var scrapedEvent = await _boylesportsHandler.Handle(@event);

                if (ScrapedEventsCatalog[Bookmaker.Boylesports].ContainsKey(DateTime.Today))
                {
                    if (ScrapedEventsCatalog[Bookmaker.Boylesports][DateTime.Today]
                        .ContainsKey($"{scrapedEvent.Name}{scrapedEvent.StartTime.ToString("HHmm")}"))
                    {
                        ScrapedEventsCatalog[Bookmaker.Boylesports][DateTime.Today]
                            [$"{scrapedEvent.Name}{scrapedEvent.StartTime.ToString("HHmm")}"] = scrapedEvent;
                    }
                    else
                    {
                        ScrapedEventsCatalog[Bookmaker.Boylesports][DateTime.Today]
                            .Add($"{scrapedEvent.Name}{scrapedEvent.StartTime.ToString("HHmm")}", scrapedEvent);
                    }
                }
                else
                {
                    ScrapedEventsCatalog[Bookmaker.Boylesports] = new Dictionary<DateTime, Dictionary<string, ScrapedEvent>>();
                    ScrapedEventsCatalog[Bookmaker.Boylesports].Add(DateTime.Today, new Dictionary<string, ScrapedEvent>() {
                        { $"{scrapedEvent.Name}{scrapedEvent.StartTime.ToString("HHmm")}", scrapedEvent } });
                }
            }
        }

        public List<ScrapedEvent> GetScrapedEvents(Bookmaker bookmaker, DateTime dateTime)
        {
            return ScrapedEventsCatalog[bookmaker][dateTime].Values.ToList();
        }
    }
}

