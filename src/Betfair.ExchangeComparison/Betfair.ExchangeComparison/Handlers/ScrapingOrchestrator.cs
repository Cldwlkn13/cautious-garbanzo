using System.Collections.Concurrent;
using Betfair.ExchangeComparison.Domain.Definitions.Base;
using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Interfaces;
using Betfair.ExchangeComparison.Scraping.Boylesports.Interfaces;
using Betfair.ExchangeComparison.Scraping.Interfaces;
using Betfair.ExchangeComparison.Scraping.Oddschecker.Interfaces;
using Betfair.ExchangeComparison.Scraping.WilliamHill.Interfaces;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Handlers
{
    public class ScrapingOrchestrator<T> : IScrapingOrchestrator<T>, IScrapingControl<T> where T : ISportBase
    {
        private readonly IBoylesportsHandler<T> _boylesportsHandler;
        private readonly IOddscheckerHandler<T> _oddscheckerHandler;
        private readonly IWilliamHillHandler<T> _williamHillHandler;

        public Dictionary<Provider, bool> SwitchBoard { get; private set; }
        public Dictionary<Provider, DateTime> Expiries { get; private set; }

        public ScrapingOrchestrator(IBoylesportsHandler<T> boylesportsHandler, IOddscheckerHandler<T> oddscheckerHandler, IWilliamHillHandler<T> williamHillHandler)
        {
            _boylesportsHandler = boylesportsHandler;
            _oddscheckerHandler = oddscheckerHandler;
            _williamHillHandler = williamHillHandler;

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

        public async Task Orchestrate(IEnumerable<MarketDetailWithEvent> catalog, Provider provider)
        {
            switch (provider)
            {
                case Provider.BoylesportsDirect:
                    await OrchestrateSingleEvents(catalog, provider);
                    break;
                case Provider.WilliamHillDirect:
                    await OrchestrateEnumerable(catalog, provider);
                    break;
            }
        }

        public async Task Orchestrate(ExchangeCatalogue catalog, Provider provider)
        {
            switch (provider)
            {
                case Provider.Oddschecker:
                case Provider.WilliamHillDirect:
                    await OrchestrateEnumerable(catalog, provider);
                    break;
            }
        }

        public async Task Orchestrate(Dictionary<EventWithCompetition, List<MarketDetail>> catalog, Provider provider)
        {
            switch (provider)
            {
                case Provider.WilliamHillDirect:
                    await OrchestrateEnumerable(catalog, provider);
                    break;
            }
        }

        private async Task OrchestrateSingleEvents(IEnumerable<MarketDetailWithEvent> catalog, Provider provider)
        {
            CheckProvider(provider);

            IScrapingHandler<T> _handler = ResolveHandler(provider);

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

        private async Task OrchestrateEnumerable(IEnumerable<MarketDetailWithEvent> catalog, Provider provider)
        {
            CheckProvider(provider);

            IScrapingHandler<T> _handler = ResolveHandler(provider);

            var scrapedEvents = await _handler.HandleEnumerable(catalog);

            foreach (var scrapedEvent in scrapedEvents)
            {
                MaintainCatalogue(provider, scrapedEvent);
            }
        }

        private async Task OrchestrateEnumerable(Dictionary<EventWithCompetition, List<MarketDetail>> catalog, Provider provider)
        {
            CheckProvider(provider);

            IScrapingHandler<T> _handler = ResolveHandler(provider);

            var scrapedEvents = await _handler.HandleEnumerable(catalog);

            foreach (var scrapedEvent in scrapedEvents)
            {
                MaintainCatalogue(provider, scrapedEvent);
            }
        }

        private async Task OrchestrateEnumerable(ExchangeCatalogue catalog, Provider provider)
        {
            //CheckProvider(provider);

            //IScrapingHandler<T> _handler = ResolveHandler(provider);

            //var competitions = catalog.MarketCatalogues.GroupBy(m => new EventByCountry
            //{
            //    CountryCode = m.Event.CountryCode,
            //    CompetitionName = m.Competition.Name,
            //    EventName = m.Event.Name,
            //    EventStartTime = m.Description.MarketTime
            //});

            //EventsByCountry eventsByCountry = new EventsByCountry
            //{
            //    Events = competitions.GroupBy(c => c.Key.CountryCode)
            //};

            //var scrapedEvents = await _handler.HandleEnumerable(eventsByCountry);

            //foreach (var scrapedEvent in scrapedEvents)
            //{
            //    var comps = catalog.MarketCatalogues.Select(m => m.Competition);

            //    var mappedEvent = catalog.EventDictionary.FirstOrDefault(e => e.Value.Name == scrapedEvent.Name).Value;
            //    var mappedEbc = competitions.First(c => c.Key.EventName == mappedEvent.Name).Key;
            //    var mappedCompetition = comps.FirstOrDefault(c => c.Name == mappedEbc.CompetitionName);
            //    var ewc = new EventWithCompetition
            //    {
            //        Competition = mappedCompetition!,
            //        Event = mappedEvent
            //    };

            //    var marketCatalogues = catalog.MarketCatalogues.Where(m => m.Event.Id == mappedEvent.Id);

            //    scrapedEvent.MappedEventWithMarketCatalogues = new MarketCataloguesWithEvent()
            //    {
            //        EventWithCompetition = ewc,
            //        ExchangeMarketCatalogues = marketCatalogues
            //    };

            //    var maps = CommonWilliamHillExtensions.WilliamHillMarketTypeMaps();

            //    foreach (var scrapedMarket in scrapedEvent.ScrapedMarkets)
            //    {
            //        scrapedMarket.Name = maps[scrapedMarket.Name];
            //    }

            //    MaintainCatalogue(provider, scrapedEvent);
            //}
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

        private IScrapingHandler<T> ResolveHandler(Provider provider)
        {
            switch (provider)
            {
                case Provider.BoylesportsDirect:
                    return _boylesportsHandler;
                case Provider.WilliamHillDirect:
                    return _williamHillHandler;
                case Provider.Oddschecker:
                    return _oddscheckerHandler;
                default:
                    return _oddscheckerHandler;
            }
        }
    }
}

