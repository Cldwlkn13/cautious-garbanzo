using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Scraping.Interfaces;
using Betfair.ExchangeComparison.Scraping.WilliamHill.Interfaces;
using Betfair.ExchangeComparison.Sportsbook.Model;
using Microsoft.Extensions.Logging;

namespace Betfair.ExchangeComparison.Scraping.WilliamHill
{
    public class WilliamHillHandlerRacing<T> : ScrapingHandler<T>, IWilliamHillHandler<T>
    {
        private const string BaseUrl = "https://sports.williamhill.com";
        private IWilliamHillParser<T> _parser;

        public WilliamHillHandlerRacing(ILogger<WilliamHillHandlerRacing<T>> logger, IScrapingClient scrapingClient, IWilliamHillParser<T> parser) :
            base(logger, scrapingClient)
        {
            _parser = parser;
        }

        public async Task<IEnumerable<ScrapedEvent>> HandleEnumerable(Sport sport = Sport.Racing)
        {
            var basePage = $"{BaseUrl}/betting/en-gb/horse-racing/meetings/all/today";

            var html = await _scrapingClient.ScrapeZenRowsAsync(basePage,
                new Dictionary<string, string> {
                        { "premium_proxy", "true" },
                        { "proxy_country", "gb" }
            });

            //var html = LoadFromFile("test-meetings-html.txt");

            if (string.IsNullOrEmpty(html))
            {
                _logger.LogWarning($"HTML_EMPTY; Can not continue parsing empty string. url={basePage}");
                return new List<ScrapedEvent>();
            }

            var links = _parser.BuildHorseRacingLinks(html);

            //var meetingHtml = LoadFromFile("test-single-meeting-html.txt");

            var meetingHtml = await _scrapingClient.ScrapeZenRowsAsync($"{BaseUrl}{links["Curragh"]}",
                new Dictionary<string, string> {
                        { "premium_proxy", "true" },
                        { "proxy_country", "gb" },
            });

            var splitRaces = _parser.SplitRaces(meetingHtml);

            foreach (var raceNode in splitRaces)
            {
                var scrapedEvent = _parser.BuildScrapedEvent(raceNode);
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

        public Task<IEnumerable<ScrapedEvent>> HandleEnumerable(EventsByCountry ebc)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ScrapedEvent>> HandleEnumerable(IEnumerable<MarketDetailWithEvent> events, Sport sport = Sport.Racing)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ScrapedEvent>> HandleEnumerable(Dictionary<EventWithCompetition, List<MarketDetail>> events, Sport sport = Sport.Racing)
        {
            throw new NotImplementedException();
        }
    }
}

