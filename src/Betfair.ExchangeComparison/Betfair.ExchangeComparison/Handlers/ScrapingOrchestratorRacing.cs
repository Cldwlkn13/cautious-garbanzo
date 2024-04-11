using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Interfaces;
using Betfair.ExchangeComparison.Scraping.Interfaces;
using Betfair.ExchangeComparison.Scraping.Oddschecker.Interfaces;
using System.Collections.Concurrent;

namespace Betfair.ExchangeComparison.Handlers
{
    public class ScrapingOrchestratorRacing : IScrapingOrchestratorRacing, IScrapingControlRacing 
    {
        private readonly IOddscheckerHandlerRacing _oddscheckerHandler;
        private readonly IUsageHandler _usageHandler;

        public Dictionary<Provider, bool> SwitchBoard { get; private set; }
        public Dictionary<Provider, DateTime> Expiries { get; private set; }

        public ScrapingOrchestratorRacing(IOddscheckerHandlerRacing oddscheckerHandler, IUsageHandler usageHandler)
        {
            _oddscheckerHandler = oddscheckerHandler;
            _usageHandler = usageHandler;

            SwitchBoard = new Dictionary<Provider, bool>();
            Expiries = new Dictionary<Provider, DateTime>();
            foreach (var provider in Enum.GetValues(typeof(Provider)))
            {
                SwitchBoard.Add((Provider)provider, false);
                Expiries.Add((Provider)provider, DateTime.UtcNow);
            }

            ScrapedEventsCatalog = new ConcurrentDictionary<Provider, ConcurrentDictionary<DateTime, ConcurrentDictionary<string, ScrapedEvent>>>();
        }

        public ConcurrentDictionary<Provider, ConcurrentDictionary<DateTime, ConcurrentDictionary<string, ScrapedEvent>>> ScrapedEventsCatalog { get; set; }

        public void Start(Provider provider)
        {
            SwitchBoard[provider] = true;
            Expiries[provider] = DateTime.UtcNow.AddMinutes(10);
            Console.WriteLine($"Starting {provider} scraping!");
            Console.WriteLine($"Set Expiry on {provider} scraping to {Expiries[provider]}");
        }

        public void Stop(Provider provider)
        {
            SwitchBoard[provider] = false;
            Expiries[provider] = DateTime.UtcNow;
            Console.WriteLine($"Stopping {provider} scraping!");
        }

        public void UpdateExpiry(Provider provider)
        {
            Expiries[provider] = DateTime.UtcNow.AddMinutes(10);
            Console.WriteLine($"Update Expiry on {provider} scraping to {Expiries[provider]}");
        }

        public async Task Orchestrate(IEnumerable<MarketDetailWithEwc> catalog, Provider provider)
        {
            switch (provider)
            {
                case Provider.BoylesportsDirect:
                    await OrchestrateSingleEvents(catalog, provider);
                    break;
                case Provider.Oddschecker:
                    await OrchestrateSingleEvents(catalog, provider);
                    break;
            }
        }

        private async Task OrchestrateSingleEvents(IEnumerable<MarketDetailWithEwc> catalog, Provider provider)
        {
            CheckProvider(provider);

            IScrapingHandlerSingle _handler = ResolveHandler(provider);

            Parallel.ForEach(catalog, new ParallelOptions { MaxDegreeOfParallelism = 10 }, @event =>
            {
                var scrapedEvent = _handler.Handle(@event).Result;

                if (scrapedEvent.MappedEventWithCompetition == null)
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

        private void CheckProvider(Provider provider)
        {
            if (!ScrapedEventsCatalog.ContainsKey(provider))
            {
                ScrapedEventsCatalog.AddOrUpdate(provider,
                    new ConcurrentDictionary<DateTime, ConcurrentDictionary<string, ScrapedEvent>>(),
                    (k, v) => v = new ConcurrentDictionary<DateTime, ConcurrentDictionary<string, ScrapedEvent>>());
            }
        }

        private void MaintainCatalogue(Provider provider, ScrapedEvent scrapedEvent)
        {
            ClearOldData();

            if (ScrapedEventsCatalog[provider].ContainsKey(DateTime.Today))
            {
                if (ScrapedEventsCatalog[provider][DateTime.Today]
                    .ContainsKey($"{scrapedEvent.BetfairName}{scrapedEvent.StartTime.ToString("HHmm")}"))
                {
                    ScrapedEventsCatalog[provider][DateTime.Today]
                        [$"{scrapedEvent.BetfairName}{scrapedEvent.StartTime.ToString("HHmm")}"] = scrapedEvent;
                }
                else
                {
                    ScrapedEventsCatalog[provider][DateTime.Today]
                        .AddOrUpdate($"{scrapedEvent.BetfairName}{scrapedEvent.StartTime.ToString("HHmm")}",
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
                    .AddOrUpdate($"{scrapedEvent.BetfairName}{scrapedEvent.StartTime.ToString("HHmm")}",
                    scrapedEvent,
                    (k, v) => v = scrapedEvent);
            }
        }

        public async Task<UsageModel> Usage()
        {
            return await _usageHandler.GetUsage();
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

        private IScrapingHandlerSingle ResolveHandler(Provider provider)
        {
            return provider switch
            {
                Provider.Oddschecker => _oddscheckerHandler,
                _ => _oddscheckerHandler,
            };
        }
    }
}

