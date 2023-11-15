using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Scraping.Boylesports.Interfaces;
using Betfair.ExchangeComparison.Scraping.Interfaces;
using Betfair.ExchangeComparison.Sportsbook.Model;
using Microsoft.Extensions.Logging;

namespace Betfair.ExchangeComparison.Scraping.Boylesports
{
    public class BoylesportsHandler<T> : ScrapingHandler<T>, IBoylesportsHandler<T>
    {
        private readonly IBoylesportsParser<T> _parser;

        public BoylesportsHandler(ILogger<BoylesportsHandler<T>> logger, IScrapingClient scrapingClient, IBoylesportsParser<T> parser) :
            base(logger, scrapingClient)
        {
            _parser = parser;
        }

        public async Task<ScrapedEvent> Handle(MarketDetailWithEvent @event, Sport sport = Sport.Racing)
        {
            var url = UrlBuilder(@event);

            var html = await _scrapingClient.ScrapeZenRowsAsync(url, new Dictionary<string, string> { { "js_render", "true" } });

            if (string.IsNullOrEmpty(html))
            {
                _logger.LogWarning($"HTML_EMPTY; Can not continue parsing empty string. url={url}");
                return new ScrapedEvent();
            }

            var scrapedEvent = _parser.BuildScrapedEvent(html, @event);

            Console.WriteLine($"Event={@event.EventWithCompetition.Event.Name} " +
                $"{@event.SportsbookMarket.marketStartTime} successfully scraped!");

            return scrapedEvent;
        }

        private static string UrlBuilder(MarketDetailWithEvent compoundObj)
        {
            string baseUrl = "https://www.boylesports.com/sports/horse-racing/uk-ire-featured"; //e.g. 201023/redcar/13:35

            var name = compoundObj.EventWithCompetition.Event.Venue;

            var date = compoundObj.SportsbookMarket.marketStartTime
                .ConvertUtcToBritishIrishLocalTime()
                .ToString("ddMMyy");

            var time = compoundObj.SportsbookMarket.marketStartTime
                .ConvertUtcToBritishIrishLocalTime()
                .ToString("HH:mm");

            var url = $"{baseUrl}/{date}/{name}/{time}";

            return url;
        }

        public async Task<UsageModel> Usage() =>
            await _scrapingClient.ZenRowsUsage();

        public Task<ScrapedEvent> Handle(Sport sport = Sport.Racing)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ScrapedEvent>> HandleEnumerable(Sport sport = Sport.Racing)
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

