using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Domain.ScrapingModel.Oddschecker;
using Betfair.ExchangeComparison.Scraping.Extensions;
using Betfair.ExchangeComparison.Scraping.Interfaces;
using Betfair.ExchangeComparison.Scraping.Oddschecker.Interfaces;
using Betfair.ExchangeComparison.Sportsbook.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Scraping.Oddschecker.Football
{
    public class OddscheckerHandlerFootball : OddscheckerHandler, IOddscheckerHandlerFootball
    {
        private readonly IOddscheckerParserFootball _parser;

        public OddscheckerHandlerFootball(ILogger<OddscheckerHandlerFootball> logger, IScrapingClient scrapingClient, IOddscheckerParserFootball parser) :
            base(logger, scrapingClient)
        {
            _parser = parser;
        }

        public async Task<IEnumerable<ScrapedEvent>> HandleEnumerable(EventsByCountry ebc)
        {
            var scrapedEvents = new List<ScrapedEvent>();
            string baseUrl = "https://www.oddschecker.com/football";

            var countryCode = ebc.Events.FirstOrDefault(e => e.Key == "TR").ToList();
            if (countryCode == null)
            {
                return new List<ScrapedEvent>();
            }

            var competitions = countryCode.GroupBy(c => c.Key.CompetitionName);

            foreach (var competition in competitions)
            {
                var cntryCode = competition.First().Key.CountryCode;

                if (!OddscheckerMappingExtensions.TryMapCountry(cntryCode, out CountryMapping cm))
                {
                    continue;
                }
                if (!competition.Key.TryMapCompetition(cntryCode, out var mappedCompetition))
                {
                    continue;
                }

                var url = $"{baseUrl}/{cm.CountryName}/{mappedCompetition}";

                var competitionHtml = await _scrapingClient.ScrapeZenRowsAsync(url,
                    new Dictionary<string, string>());

                //var competitionHtml = LoadFromFile("TestHtml/competition-page-html.txt");

                if (string.IsNullOrEmpty(competitionHtml))
                {
                    _logger.LogWarning($"HTML_EMPTY; Can not continue parsing empty string. url={url}");
                    return new List<ScrapedEvent>();
                }

                var matchLinks = _parser.ParseLinksFromCompetitionPageSimple(competitionHtml);

                if (matchLinks == null || !competition.Any())
                {
                    Console.WriteLine($"{cm.CountryName}/{mappedCompetition} Trying Complex Parsing");
                }

                foreach (var link in matchLinks)
                {
                    if (!ScrapingExtensions.TryMapEventByParticipantNames(link,
                        competition.ToList().Select(g => g.Key),
                        out var mappedEbc,
                        minDistance: 10))
                    {
                        continue;
                    }

                    var matchUrl = $"{baseUrl}/{cm.CountryName}/{mappedCompetition}/{link.Key}/winner";

                    var matchHtml = await _scrapingClient.ScrapeZenRowsAsync(matchUrl,
                        new Dictionary<string, string>());

                    //var matchHtml = LoadFromFile("TestHtml/match-football.txt");

                    if (string.IsNullOrEmpty(matchHtml))
                    {
                        _logger.LogWarning($"HTML_EMPTY; Can not continue parsing empty string. url={url}");
                        return new List<ScrapedEvent>();
                    }

                    var ocMarketDefinitions = _parser.BuildMarketDefinitions(matchHtml);

                    var desiredMarketDefinitions = ocMarketDefinitions.Where(m =>
                        m.OcMarketTypeName == "Win Market" ||
                        m.OcMarketTypeName == "Total Goals Over/Under" ||
                        m.OcMarketTypeName == "Both Teams To Score");

                    List<OcMarket> ocMarkets = new List<OcMarket>();
                    foreach (var marketDef in desiredMarketDefinitions)
                    {
                        var apiUrl = $"https%3A%2F%2Fwww.oddschecker.com%2Fapi%2Fmarkets%2Fv2%2Fall-odds%3F" +
                            $"market-ids%3D{marketDef!.OcMarketId}%26repub%3DOC";

                        var marketJson = await _scrapingClient.ScrapeZenRowsAsync(apiUrl,
                            new Dictionary<string, string>());

                        var ocMarket = JsonConvert.DeserializeObject<OcMarket[]>(marketJson);

                        ocMarkets.Add(ocMarket.First());
                    }

                    var scrapedEvent = _parser.BuildScrapedEventFromJson(ocMarkets, mappedEbc);

                    scrapedEvents.Add(scrapedEvent);
                }
            }

            return scrapedEvents;
        }

        public Task<IEnumerable<ScrapedEvent>> HandleEnumerable(Dictionary<EventWithCompetition, List<MarketDetail>> events)
        {
            throw new NotImplementedException();
        }
    }
}

