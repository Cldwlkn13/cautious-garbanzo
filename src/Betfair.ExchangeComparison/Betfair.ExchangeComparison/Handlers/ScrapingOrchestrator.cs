using System.Collections.Concurrent;
using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Interfaces;
using Betfair.ExchangeComparison.Scraping.Boylesports.Interfaces;
using Betfair.ExchangeComparison.Scraping.Interfaces;
using Betfair.ExchangeComparison.Scraping.Oddschecker.Interfaces;
using Betfair.ExchangeComparison.Scraping.WilliamHill.Interfaces;

namespace Betfair.ExchangeComparison.Handlers
{
    public class ScrapingOrchestrator : IScrapingOrchestrator, IScrapingControl
    {
        private readonly IBoylesportsHandler _boylesportsHandler;
        private readonly IOddscheckerHandler _oddscheckerHandler;

        public Dictionary<Provider, bool> SwitchBoard { get; private set; }

        public ScrapingOrchestrator(IBoylesportsHandler boylesportsHandler, IOddscheckerHandler oddscheckerHandler)
        {
            _boylesportsHandler = boylesportsHandler;
            _oddscheckerHandler = oddscheckerHandler;

            SwitchBoard = new Dictionary<Provider, bool>();
            foreach (var provider in Enum.GetValues(typeof(Provider)))
            {
                SwitchBoard.Add((Provider)provider, false);
            }

            ScrapedEventsCatalog = new ConcurrentDictionary<Provider, ConcurrentDictionary<DateTime, ConcurrentDictionary<string, ScrapedEvent>>>();
        }

        public ConcurrentDictionary<Provider, ConcurrentDictionary<DateTime, ConcurrentDictionary<string, ScrapedEvent>>> ScrapedEventsCatalog { get; set; }

        public void Start(Provider provider)
        {
            SwitchBoard[provider] = true;
        }

        public void Stop(Provider provider)
        {
            SwitchBoard[provider] = false;
        }

        public async Task Orchestrate(IEnumerable<MarketDetailWithEvent> catalog, Provider provider)
        {
            if (!ScrapedEventsCatalog.ContainsKey(provider))
            {
                ScrapedEventsCatalog.AddOrUpdate(provider, new ConcurrentDictionary<DateTime, ConcurrentDictionary<string, ScrapedEvent>>(),
                    (k, v) => v = new ConcurrentDictionary<DateTime, ConcurrentDictionary<string, ScrapedEvent>>());
            }

            IScrapingHandler _handler = ResolveHandler(provider);

            Parallel.ForEach(catalog, new ParallelOptions { MaxDegreeOfParallelism = 10 }, @event =>
            {

                var scrapedEvent = _handler.Handle(@event).Result;

                if (scrapedEvent.MappedEvent == null)
                {
                    return;
                }

                MaintainCatalogue(provider, scrapedEvent);

            });
        }

        public bool TryGetScrapedEvents(Provider provider, DateTime dateTime, out List<ScrapedEvent> result)
        {
            result = new List<ScrapedEvent>();

            if (!ScrapedEventsCatalog.ContainsKey(provider))
            {
                return false;
            }

            if (!ScrapedEventsCatalog[provider].ContainsKey(dateTime))
            {
                return false;
            }

            result = ScrapedEventsCatalog[provider][dateTime].Values.ToList();

            return true;
        }

        private void MaintainCatalogue(Provider provider, ScrapedEvent scrapedEvent)
        {
            ClearOldData();

            if (ScrapedEventsCatalog[provider].ContainsKey(DateTime.Today))
            {
                if (ScrapedEventsCatalog[provider][DateTime.Today]
                    .ContainsKey($"{scrapedEvent.Name}{scrapedEvent.StartTime.ToString("HHmm")}"))
                {
                    ScrapedEventsCatalog[provider][DateTime.Today]
                        [$"{scrapedEvent.Name}{scrapedEvent.StartTime.ToString("HHmm")}"] = scrapedEvent;
                }
                else
                {
                    ScrapedEventsCatalog[provider][DateTime.Today]
                        .AddOrUpdate($"{scrapedEvent.Name}{scrapedEvent.StartTime.ToString("HHmm")}",
                        scrapedEvent,
                        (k, v) => v = scrapedEvent);
                }
            }
            else
            {
                ScrapedEventsCatalog[provider] = new ConcurrentDictionary<DateTime, ConcurrentDictionary<string, ScrapedEvent>>();
                ScrapedEventsCatalog[provider].AddOrUpdate(DateTime.Today,
                    new ConcurrentDictionary<string, ScrapedEvent>() { },
                    (k, v) => v = new ConcurrentDictionary<string, ScrapedEvent>() { });

                ScrapedEventsCatalog[provider][DateTime.Today]
                    .AddOrUpdate($"{scrapedEvent.Name}{scrapedEvent.StartTime.ToString("HHmm")}",
                    scrapedEvent,
                    (k, v) => v = scrapedEvent);
            }
        }

        public async Task<UsageModel> Usage()
        {
            return await _boylesportsHandler.Usage();
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

                ScrapedEventsCatalog[bm.Key] = new ConcurrentDictionary<DateTime, ConcurrentDictionary<string, ScrapedEvent>>(
                    cleanedDictionary);
            }
        }

        private IScrapingHandler ResolveHandler(Provider provider)
        {
            switch (provider)
            {
                case Provider.Boylesports:
                    return _boylesportsHandler;
                case Provider.Oddschecker:
                    return _oddscheckerHandler;
                default:
                    return _oddscheckerHandler;
            }
        }
    }
}

