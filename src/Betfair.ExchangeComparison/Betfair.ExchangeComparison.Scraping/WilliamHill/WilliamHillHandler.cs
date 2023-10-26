using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Scraping.Boylesports;
using Betfair.ExchangeComparison.Scraping.Interfaces;
using Betfair.ExchangeComparison.Scraping.WilliamHill.Interfaces;
using Microsoft.Extensions.Logging;

namespace Betfair.ExchangeComparison.Scraping.WilliamHill
{
    public class WilliamHillHandler : ScrapingHandler, IWilliamHillHandler
    {
        private const string BaseUrl = "https://sports.williamhill.com";
        private IWilliamHillParser _customParser;

        public WilliamHillHandler(ILogger<BoylesportsHandler> logger, IScrapingClient scrapingClient, IWilliamHillParser parser, IWilliamHillParser customParser) :
            base(logger, scrapingClient, parser)
        {
            _customParser = customParser;
        }

        public async Task<IEnumerable<ScrapedEvent>> HandleEnumerable(Sport sport = Sport.Racing)
        {
            if (sport == Sport.Racing)
            {
                var basePage = $"{BaseUrl}/betting/en-gb/horse-racing/meetings";

                //var html = await _scrapingClient.ScrapeZenRowsAsync(basePage,
                //    new Dictionary<string, string> {
                //        { "js_render", "true" },
                //        { "premium_proxy", "true" },
                //        { "proxy_country", "gb" }
                //});

                var html = LoadFromFile("test-meetings-html.txt");

                if (string.IsNullOrEmpty(html))
                {
                    _logger.LogWarning($"HTML_EMPTY; Can not continue parsing empty string. url={basePage}");
                    return new List<ScrapedEvent>();
                }

                var links = _customParser.BuildHorseRacingLinks(html);

                //var meetingHtml = LoadFromFile("test-single-meeting-html.txt");

                var meetingHtml = await _scrapingClient.ScrapeZenRowsAsync($"{BaseUrl}{links["Kempton"]}",
                    new Dictionary<string, string> {
                        { "js_render", "true" },
                        { "premium_proxy", "true" },
                        { "proxy_country", "gb" },
                        { "wait_for", ".outermain" },
                        { "wait", "10000" }
                });

                var splitRaces = _customParser.SplitRaces(meetingHtml);

                foreach (var raceNode in splitRaces)
                {
                    var scrapedEvent = _customParser.BuildScrapedEvent(raceNode);
                }
            }

            return new List<ScrapedEvent>();
        }

        private static string LoadFromFile(string path)
        {
            string html = "";
            if (File.Exists(path))
            {
                html = File.ReadAllText(path);
            }
            else
            {
                Console.WriteLine("The file does not exist in the current folder.");
            }

            return html;
        }

        private static string UrlBuilder(Sport sport)
        {
            string footballBaseUrl = $"{BaseUrl}/football/matches/competition/today/";
            string racingBaseUrl = $"{BaseUrl}/horse-racing/meetings/";

            //var name = compoundObj.EventWithCompetition.Event.Venue;

            //var date = compoundObj.SportsbookMarket.marketStartTime
            //    .ConvertUtcToBritishIrishLocalTime()
            //    .ToString("ddMMyy");

            //var time = compoundObj.SportsbookMarket.marketStartTime
            //    .ConvertUtcToBritishIrishLocalTime()
            //    .ToString("HH:mm");

            //var url = $"{baseUrl}/{date}/{name}/{time}";

            return "";
        }

        public async Task<UsageModel> Usage() =>
            await _scrapingClient.ZenRowsUsage();

        public Task<ScrapedEvent> Handle(MarketDetailWithEvent @event, Sport sport = Sport.Racing)
        {
            throw new NotImplementedException();
        }

        public async Task<ScrapedEvent> Handle(Sport sport = Sport.Racing)
        {
            throw new NotImplementedException();
        }
    }
}

