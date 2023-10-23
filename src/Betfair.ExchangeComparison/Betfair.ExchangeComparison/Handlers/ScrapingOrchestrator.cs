using System.Collections.Concurrent;
using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Interfaces;
using Betfair.ExchangeComparison.Scraping.Boylesports.Interfaces;
using Betfair.ExchangeComparison.Scraping.Interfaces;

namespace Betfair.ExchangeComparison.Handlers
{
    public class ScrapingOrchestrator : IScrapingOrchestrator, IScrapingControl
    {
        private readonly IBoylesportsHandler _boylesportsHandler;

        public Dictionary<Bookmaker, bool> SwitchBoard { get; private set; }

        public ScrapingOrchestrator(IBoylesportsHandler boylesportsHandler)
        {
            _boylesportsHandler = boylesportsHandler;

            SwitchBoard = new Dictionary<Bookmaker, bool>();
            foreach (var bookmaker in Enum.GetValues(typeof(Bookmaker)))
            {
                SwitchBoard.Add((Bookmaker)bookmaker, false);
            }

            ScrapedEventsCatalog = new ConcurrentDictionary<Bookmaker, ConcurrentDictionary<DateTime, ConcurrentDictionary<string, ScrapedEvent>>>();
        }

        public ConcurrentDictionary<Bookmaker, ConcurrentDictionary<DateTime, ConcurrentDictionary<string, ScrapedEvent>>> ScrapedEventsCatalog { get; set; }

        public void Start(Bookmaker bookmaker)
        {
            SwitchBoard[bookmaker] = true;
        }

        public void Stop(Bookmaker bookmaker)
        {
            SwitchBoard[bookmaker] = false;
        }

        public async Task Orchestrate(IEnumerable<MarketDetailWithEvent> catalog, Bookmaker bookmaker)
        {
            if (!ScrapedEventsCatalog.ContainsKey(bookmaker))
            {
                ScrapedEventsCatalog.AddOrUpdate(bookmaker, new ConcurrentDictionary<DateTime, ConcurrentDictionary<string, ScrapedEvent>>(),
                    (k, v) => v = new ConcurrentDictionary<DateTime, ConcurrentDictionary<string, ScrapedEvent>>());
            }

            IScrapingHandler _handler = ResolveHandler(bookmaker);

            Parallel.ForEach(catalog, new ParallelOptions { MaxDegreeOfParallelism = 10 }, @event =>
            {
                var scrapedEvent = _handler.Handle(@event).Result;

                if (scrapedEvent.MappedEvent == null)
                {
                    return;
                }

                MaintainCatalogue(bookmaker, scrapedEvent);
            });

            await _boylesportsHandler.Usage();
        }

        public bool TryGetScrapedEvents(Bookmaker bookmaker, DateTime dateTime, out List<ScrapedEvent> result)
        {
            result = new List<ScrapedEvent>();

            if (!ScrapedEventsCatalog.ContainsKey(bookmaker))
            {
                return false;
            }

            if (!ScrapedEventsCatalog[bookmaker].ContainsKey(dateTime))
            {
                return false;
            }

            result = ScrapedEventsCatalog[bookmaker][dateTime].Values.ToList();

            return true;
        }

        private void MaintainCatalogue(Bookmaker bookmaker, ScrapedEvent scrapedEvent)
        {
            ClearOldData();

            if (ScrapedEventsCatalog[bookmaker].ContainsKey(DateTime.Today))
            {
                if (ScrapedEventsCatalog[bookmaker][DateTime.Today]
                    .ContainsKey($"{scrapedEvent.Name}{scrapedEvent.StartTime.ToString("HHmm")}"))
                {
                    ScrapedEventsCatalog[bookmaker][DateTime.Today]
                        [$"{scrapedEvent.Name}{scrapedEvent.StartTime.ToString("HHmm")}"] = scrapedEvent;
                }
                else
                {
                    ScrapedEventsCatalog[bookmaker][DateTime.Today]
                        .AddOrUpdate($"{scrapedEvent.Name}{scrapedEvent.StartTime.ToString("HHmm")}",
                        scrapedEvent,
                        (k, v) => v = scrapedEvent);
                }
            }
            else
            {
                ScrapedEventsCatalog[bookmaker] = new ConcurrentDictionary<DateTime, ConcurrentDictionary<string, ScrapedEvent>>();
                ScrapedEventsCatalog[bookmaker].AddOrUpdate(DateTime.Today,
                    new ConcurrentDictionary<string, ScrapedEvent>() { },
                    (k, v) => v = new ConcurrentDictionary<string, ScrapedEvent>() { });

                ScrapedEventsCatalog[bookmaker][DateTime.Today]
                    .AddOrUpdate($"{scrapedEvent.Name}{scrapedEvent.StartTime.ToString("HHmm")}",
                    scrapedEvent,
                    (k, v) => v = scrapedEvent);
            }
        }

        private void ClearOldData()
        {
            foreach (var bm in ScrapedEventsCatalog)
            {
                if (!ScrapedEventsCatalog[bm.Key]
                    .Any(k => k.Key < DateTime.Today))
                {
                    continue;
                }

                var cleanedDictionary = ScrapedEventsCatalog[bm.Key]
                    .Where(k => k.Key >= DateTime.Today)
                    .ToDictionary(k => k.Key, v => v.Value);

                ScrapedEventsCatalog[bm.Key] = new ConcurrentDictionary<DateTime, ConcurrentDictionary<string, ScrapedEvent>>(cleanedDictionary);
            }
        }

        private IScrapingHandler ResolveHandler(Bookmaker bookmaker)
        {
            switch (bookmaker)
            {
                case Bookmaker.Boylesports:
                    return _boylesportsHandler;
                default:
                    return _boylesportsHandler;
            }
        }
    }
}

