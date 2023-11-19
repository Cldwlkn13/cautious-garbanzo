using System.Collections.Concurrent;
using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Domain.ScrapingModel.WilliamHill;
using Betfair.ExchangeComparison.Scraping.Extensions;
using Betfair.ExchangeComparison.Scraping.Interfaces;
using Betfair.ExchangeComparison.Scraping.WilliamHill.Interfaces;
using Betfair.ExchangeComparison.Sportsbook.Model;
using DuoVia.FuzzyStrings;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Scraping.WilliamHill.Football
{
    public class WilliamHillHandlerFootball<T> : ScrapingHandler<T>, IWilliamHillHandler<T>
    {
        private const string BaseUrl = "https://sports.williamhill.com";
        private IWilliamHillParser<T> _parser;

        public WilliamHillHandlerFootball(ILogger<WilliamHillHandlerRacing<T>> logger, IScrapingClient scrapingClient, IWilliamHillParser<T> parser) :
            base(logger, scrapingClient)
        {
            _parser = parser;

        }

        public async Task<ScrapedEvent> Handle(Sport sport = Sport.Racing)
        {
            throw new NotImplementedException();
        }

        public Task<ScrapedEvent> Handle(MarketDetailWithEvent @event, Sport sport = Sport.Racing)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ScrapedEvent>> HandleEnumerable(IEnumerable<MarketDetailWithEvent> mdes, Sport sport = Sport.Racing)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ScrapedEvent>> HandleEnumerable(EventsByCountry ebc)
        {
            throw new NotImplementedException();
        }

        public Task<UsageModel> Usage()
        {
            throw new NotImplementedException();
        }

        private Dictionary<WhEvent, List<WhMarket>> ListEventsByMarket()
        {
            var result = new Dictionary<WhEvent, List<WhMarket>>();
            var bag = new ConcurrentBag<WhEvent>();

            //foreach (var mg in WilliamHillMappingExtensions.WilliamHillMarketGroups())
            //{
            Parallel.ForEach(WilliamHillMappingExtensions.WilliamHillMarketGroups(), mg =>
            {
                try
                {
                    var hasMoreItems = true;
                    var index = 0;

                    var @events = new List<WhEvent>();

                    while (hasMoreItems)
                    {
                        //https://sports.williamhill.com/data/dml01/en-gb/OB_SP9/date/today/match-betting?page=1
                        var url = $"{BaseUrl}/data/dml01/en-gb/OB_SP9/date/today/{mg}?page={index}";

                        var json = _scrapingClient.ScrapeZenRowsAsync(url,
                            new Dictionary<string, string>
                            {
                                { "premium_proxy", "true" },
                                { "proxy_country", "gb" }
                            }).Result;

                        if (!TryDeserialize(json, out var apiResults))
                        {
                            Console.WriteLine($"Could not deserialize Json for {mg}");

                            return;
                        }

                        if (!apiResults.data.Any())
                        {
                            return;
                        }

                        foreach (var @event in apiResults.data[0].events)
                        {
                            bag.Add(@event);
                        }

                        if (apiResults.hasMoreItems)
                        {
                            index++;
                            if (index > 3)
                            {
                                hasMoreItems = false;
                            }

                        }
                        else
                        {
                            hasMoreItems = false;
                        }
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"WilliamHill : ListEventsByMarket() Exception={exception.Message}");

                    return;
                }
            });
            //}

            var byEvent = bag.GroupBy(e => e.eventId);

            foreach (var @event in bag)
            {
                if (!result.Any(e => e.Key.eventId == @event.eventId))
                {
                    var grouping = byEvent.FirstOrDefault(e => e.Key == @event.eventId);
                    var markets = grouping.ToList().SelectMany(m => m.markets);
                    result.Add(@event, markets.ToList());
                }
            }

            return result;
        }

        private static bool TryDeserialize(string json, out WhRoot result)
        {
            result = new WhRoot();

            try
            {
                result = JsonConvert.DeserializeObject<WhRoot>(json) ??
                    throw new InvalidDataException();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Task<IEnumerable<ScrapedEvent>> HandleEnumerable(Sport sport = Sport.Racing)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ScrapedEvent>> HandleEnumerable(Dictionary<EventWithCompetition, List<MarketDetail>> events, Sport sport = Sport.Racing)
        {
            var apiResults = ListEventsByMarket();

            var result = new List<ScrapedEvent>();
            foreach (var @event in apiResults)
            {
                var dtEv = @event.Key.startDateTime;
                var max = DateTime.UtcNow.AddHours(3);
                if (@event.Key.startDateTime > DateTime.UtcNow.AddHours(3))
                {
                    continue;
                }

                var details = new KeyValuePair<string, string>(
                    @event.Key.eventName, @event.Key.startDateTime.ToString("HH:mm"));

                if (!TryMapEventByParticipantNames(
                    details,
                    events,
                    out var mapppedEwc,
                    minDistance: 10,
                    splitter: " v "))
                {
                    continue;
                }

                if (@event.Key.status == "S")
                {
                    continue;
                }

                var marketMaps = CommonWilliamHillExtensions.WilliamHillMarketTypeMaps();

                var scrapedMarkets = new List<ScrapedMarket>();
                foreach (var market in @event.Value)
                {
                    var marketsInEvent = events[mapppedEwc];

                    var mappedMarketName = marketMaps[market.marketName];

                    var mappedMarketDetail = marketsInEvent
                        .FirstOrDefault(m => mappedMarketName == m.marketName);

                    if (mappedMarketDetail == null)
                    {
                        Console.WriteLine($"Could not map market={market.marketName}");

                        continue;
                    }

                    var scrapedRunners = new List<ScrapedRunner>();
                    foreach (var selection in market.selections)
                    {
                        var runnerDetails = mappedMarketDetail!.runnerDetails;

                        if (!TryMapParticipantName(runnerDetails.Select(r => r.selectionName),
                            selection.selectionName, out var mappedName))
                        {
                            Console.WriteLine($"Could not map runner={selection.selectionName}");

                            continue;
                        }

                        var mappedRunnerDetail = runnerDetails.First(r => r.selectionName == mappedName);

                        var priceString = $"{selection.currentPriceNum}/{selection.currentPriceDen}";
                        var scrapedPrice = new ScrapedPrice(priceString, Bookmaker.WilliamHillDirect);
                        var scrapedPrices = new List<ScrapedPrice>
                        {
                            scrapedPrice
                        };

                        var scrapedRunner = new ScrapedRunner(selection.selectionName, scrapedPrices);
                        scrapedRunner.MappedRunnerDetail = mappedRunnerDetail;
                        scrapedRunners.Add(scrapedRunner);
                    }
                    var scrapedMarket = new ScrapedMarket(market.marketName, scrapedRunners);
                    scrapedMarket.MappedMarketDetail = mappedMarketDetail!;

                    scrapedMarkets.Add(scrapedMarket);
                }

                var scrapedEvent = new ScrapedEvent(scrapedMarkets);
                scrapedEvent.MappedEventWithCompetition = mapppedEwc;
                scrapedEvent.BetfairName = mapppedEwc.Event.Name;
                scrapedEvent.ScrapedEventName = @event.Key.eventName;

                scrapedEvent.StartTime = mapppedEwc.Event.OpenDate!.Value.ConvertUtcToBritishIrishLocalTime();
                result.Add(scrapedEvent);
            }

            return result;
        }

        public static bool TryMapEventByParticipantNames(KeyValuePair<string, string> scrapedDetails,
            Dictionary<EventWithCompetition, List<MarketDetail>> sportsbookEvents, out EventWithCompetition result,
            int minDistance = 10, string splitter = "-v-")
        {
            result = new EventWithCompetition();

            var cleanedBkEventName = CleanName(scrapedDetails.Key, splitter);

            var bkNamesArray = cleanedBkEventName.Split("*");

            if (bkNamesArray == null || bkNamesArray!.Length != 2)
            {
                return false;
            }

            var bkNameWords1 = Words(bkNamesArray[0], "~");
            var bkNameWords2 = Words(bkNamesArray[1], "~");

            var similarities = new Dictionary<EventWithCompetition, double>();
            foreach (var sportsbookEvent in sportsbookEvents.Where(m => m.Key.Event.OpenDate!.Value.ToString("HH:mm") == scrapedDetails.Value))
            {
                var cleanedEventName = CleanName(sportsbookEvent.Key.Event.Name, " v ");

                var bfNamesArray = cleanedEventName.Split("*");

                if (bfNamesArray == null || bfNamesArray!.Length != 2)
                {
                    continue;
                }

                var bfNameWords1 = Words(bfNamesArray[0], "~");
                var bfNameWords2 = Words(bfNamesArray[1], "~");

                var wordMatches1 = SignificantWordMatches(bkNameWords1.ToList(), bfNameWords1.ToList());
                var wordMatches2 = SignificantWordMatches(bkNameWords2.ToList(), bfNameWords2.ToList());

                double similarityA1 = bkNamesArray[0].LevenshteinDistance(bfNamesArray[0]);
                double similarityA2 = bkNamesArray[1].LevenshteinDistance(bfNamesArray[1]);

                var similarity1 = similarityA1 + similarityA2;

                //reverse
                //double similarityB1 = bkNamesArray[0].LevenshteinDistance(bfNamesArray[1]);
                //double similarityB2 = bkNamesArray[1].LevenshteinDistance(bfNamesArray[0]);

                //var similarity2 = similarityB1 + similarityB2;

                if (similarityA1 == 0 || similarityA2 == 0)
                {
                    similarities.Add(sportsbookEvent.Key, 0);

                    break;
                }
                //else if (similarityB1 == 0 || similarityB2 == 0)
                //{
                //    similarities.Add(sportsbookEvent.Key, 0);

                //    break;
                //}
                else if (wordMatches1 > 0 && wordMatches2 > 0)
                {
                    similarities.Add(sportsbookEvent.Key, 0);

                    break;
                }
                else
                {
                    //similarities.Add(sportsbookEvent.Key, Math.Min(similarity1, similarity2));

                    similarities.Add(sportsbookEvent.Key, similarity1);
                }
            }

            var min = similarities.Any() ? similarities.Min(s => s.Value) : 100;
            if (min > 5)
            {
                if (similarities.Any())
                {
                    Console.WriteLine($"Could not map Event={scrapedDetails.Key} at {scrapedDetails.Value}. " +
                        $"Best match was : {similarities.FirstOrDefault(m => m.Value == min).Key.Event.Name}; min={min}");
                }
                else
                {
                    Console.WriteLine($"Could not map Event={scrapedDetails.Key} at {scrapedDetails.Value}. ");
                }

                return false;
            }

            result = similarities.FirstOrDefault(s => s.Value == min).Key;

            Console.WriteLine($"Mapped BK Event={scrapedDetails.Key} to " +
                $"Event={result.Event.Name} at " +
                $"{result.Event.OpenDate}");

            return true;
        }

        public static bool TryMapParticipantName(IEnumerable<string> options, string dest, out string result)
        {
            result = "";

            if (dest.Contains("Draw"))
            {
                if (options.Contains("Draw"))
                {
                    result = "Draw";
                    return true;
                }
            }
            var d = new Dictionary<string, double>();
            foreach (var op in options.Where(o => o != "Draw"))
            {
                if (op.Contains(dest))
                {
                    d.Add(op, 0);
                }
                else
                {
                    d.Add(op, op.LevenshteinDistance(dest));
                }
            }

            var min = d.Values.Min();

            result = d.First(v => v.Value == min).Key;
            return true;
        }

        private static string CleanName(string name, string splitter)
        {
            return name
                    .Replace(splitter, "*")
                    .Replace("  ", " ")
                    .Replace(" ", "~")
                    .Replace("'", "")
                    .Replace(".", "")
                    .Replace("-", " ")
                    .Replace("Club", " ")
                    .Replace("FK", "")
                    .Replace("FC", "")
                    .Replace("JK", "")
                    .Replace("SC", "")
                    .Replace("ASD", "")
                    .Replace("LSK", "")
                    .Replace("CF", "")
                    .Replace("LP", "")
                    .Replace("Deportivo", "")
                    .Replace("Atletico", "")
                    .Replace("Athletico", "")
                    .Replace("IF", "")
                    .Replace("Al ", "")
                    .Replace("Al-", "")
                    .Replace("(W)", "")
                    .Replace("Women", "")
                    .Replace("(Women)", "")
                    .Replace("II", "")
                    .Replace("III", "")
                    .Replace("U18", "")
                    .Replace("U19", "")
                    .Replace("U20", "")
                    .Replace("U21", "")
                    .Replace("U22", "")
                    .Replace("U23", "")
                    .Replace("Jong", "")
                    .Replace("(Par)", "")
                    .Replace("(Uru)", "")
                    .Replace("(Col)", "")
                    .Replace("(Arg)", "")
                    .Replace("(Bra)", "")
                    .Replace("(Bol)", "")
                    .Replace("(Ecu)", "")
                    .Replace("(Ven)", "")
                    .Replace("(Mex)", "")
                    .Replace("Reserves", "")
                    .Replace("(Res)", "")
                    .ToLower()
                    .Trim()
                    .Replace(" ", "");
        }

        private static string[] Words(string name, string splitter)
        {
            return name.Split(splitter);
        }

        private static int SignificantWordMatches(List<string> a, List<string> b)
        {
            return a.Select(c => b.Any(d => d.Length >= 4 && d == c)).Count(e => e == true);
        }
    }
}

